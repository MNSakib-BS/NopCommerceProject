using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Api;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Plugin.Arch.Core.Services.Helpers;
using Nop.Services.Configuration;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
public abstract class ScheduledJobBase<TItem> : IJob
{
    private const int BatchAwaitPeriodMilliseconds = 5000;
    private const string EnableLogSettingParam = "ScheduledTaskBase_EnableLog";
    public virtual bool RunOnlyOnGlobalStore { get { return false; } }
    protected bool _enableLog;
    private bool _disposed;
    private Guid _batchKey;

    private readonly Dictionary<Guid, int> _batchTotalProcessed;
    private readonly Dictionary<Guid, int> _batchTotalConsumed;
    private readonly BlockingCollection<QueueItem<TItem>> _queue;
    private readonly CancellationTokenSource _tokenSource;
    private readonly ILogger _logger;
    private readonly IObjectConverter _objectConverter;
    private readonly string _namespace;

    private readonly static object _locker = new object();
    private readonly static HashSet<string> _runningTasks = new HashSet<string>();

    protected ArchApiSettings _archSettings;
    protected readonly IStoreService _storeService;
    protected readonly IStoreMappingService _storeMappingService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly ISettingService _settingService;
    protected readonly ILogger<ScheduledJobBase<object>> _jobLogger;
    protected readonly IStoreContext _storeContext;

    protected abstract Type TaskType { get; }
    protected abstract void CollectData();
    protected abstract void Produce();
    protected abstract void Consume(TItem item);
    protected int StoreTypeId { get; set; }
    public int RunningOnStoreId { get; private set; }
    public string StoreContextualName => $"{TaskType.FullName}-{RunningOnStoreId}";

    protected ScheduledJobBase(ISettingService settingService,
        IStoreService storeService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        ILogger logger,
        IObjectConverter objectConverter,
        ILogger<ScheduledJobBase<object>> jobLogger)
    {
        _storeService = storeService;
        _storeMappingService = storeMappingService;
        _urlRecordService = urlRecordService;
        _logger = logger;
        _objectConverter = objectConverter;
        _settingService = settingService;

        _tokenSource = new CancellationTokenSource();
        _queue = new BlockingCollection<QueueItem<TItem>>();

        _batchTotalProcessed = new Dictionary<Guid, int>();
        _batchTotalConsumed = new Dictionary<Guid, int>();

        _namespace = GetType().Namespace;
        _jobLogger = jobLogger;
        _storeContext = storeContext;
        Log("Job Constructed", null, LogLevel.Debug);
    }

    public virtual void SetStoreId(int storeId)
    {
        RunningOnStoreId = storeId;
        var storeTypeRepo = EngineContext.Current.Resolve<IRepository<StoreTypeMapping>>();
        _archSettings = _settingService.LoadSetting<ArchApiSettings>(storeId);

        var mapping = storeTypeRepo.Table.FirstOrDefault(p => p.StoreId == RunningOnStoreId);
        if (mapping != null)
        {
            Log($"Setting StoreTypeId to {mapping.StoreTypeId}", null, LogLevel.Debug);
            StoreTypeId = mapping.StoreTypeId;
        }

        storeTypeRepo = null;
    }

    [PreventConcurrentExecutionJob]
    public virtual void Execute(int storeId, bool multiStore = false)
    {
        var cancellationToken = _tokenSource.Token;
        var store = _storeService.GetStoreById(storeId);

        Log("Setting StoreId", null, LogLevel.Debug);
        SetStoreId(storeId);

        var isGlobalStore = (storeId == 0 || !multiStore) || (store != null ? store.IsGlobalStore : false);
        if (RunOnlyOnGlobalStore && !isGlobalStore)
        {
            if (!cancellationToken.IsCancellationRequested)
                OnCompleting();
            return;
        }

        if (storeId != 0 && string.IsNullOrEmpty(_archSettings.ApiEndpointAddress))
            return;

        if (IsTaskRunning())
        {
            Log($"Task already running...");
            return;
        }

        try
        {
            OnExecuting();

            Log($"Collection starting...");
            CollectData();

            Log($"Producer starting...");
            Produce();

            while (!IsBatchCompleted(_batchKey))
            {
                Log($"Waiting for batch to complete...");
                System.Threading.Tasks.Task.Delay(BatchAwaitPeriodMilliseconds, cancellationToken)
                    .GetAwaiter()
                    .GetResult();
            }
        }
        catch (Exception ex)
        {
            if (!cancellationToken.IsCancellationRequested)
                Error("An exception occured during the produce process.", ex);
        }
        finally
        {
            if (!cancellationToken.IsCancellationRequested)
                OnCompleting();
        }
    }

    private bool IsTaskRunning()
    {
        lock (_locker)
        {
            if (_runningTasks.Contains(StoreContextualName))
                return true;

            _runningTasks.Add(StoreContextualName);
            return false;
        }
    }

    protected virtual void OnExecuting()
    {
        _enableLog = GetSetting(EnableLogSettingParam, false);

        try
        {
            System.Threading.Tasks.Task.Run(() => ConsumerThread(_tokenSource.Token));

            InitBatch();
        }
        catch (Exception ex)
        {

        }
    }

    protected virtual void OnCompleting()
    {
        InternalStop("Producer completed.");
    }

    public void Stop()
    {
        InternalStop("Producer stopped.");
    }

    private void InternalStop(string logMessage)
    {
        if (!_disposed)
        {
            _disposed = true;

            _queue.CompleteAdding();
            _queue.Dispose();

            _tokenSource.Cancel();
            _tokenSource.Dispose();

            _batchTotalProcessed.Remove(_batchKey);
            _batchTotalConsumed.Remove(_batchKey);

            lock (_locker)
            {
                _runningTasks.Remove(StoreContextualName);
            }

            Log(logMessage);
        }
    }

    private void ConsumerThread(CancellationToken cancellationToken)
    {
        Log($"Consumer started...");

        while (!cancellationToken.IsCancellationRequested && !_disposed && !_queue.IsAddingCompleted)
        {
            var queueItem = QueueItem<TItem>.Default();

            try
            {
                queueItem = _queue.Take(cancellationToken); // Blocks until a new item is available
                Consume(queueItem.Item);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested && !_disposed && !_queue.IsAddingCompleted)
                {
                    Error("An exception occured during the consume process.", ex);
                    queueItem.Requeue = true;
                }
            }
            finally
            {
                try
                {
                    if (queueItem.AllowRequeue)
                    {
                        queueItem.RetryCount++;
                        _queue.Add(queueItem);
                    }
                    else if (queueItem.Requeue)
                    {
                        Log($"Failed to process item: {JsonConvert.SerializeObject(queueItem.Item)}", level: LogLevel.Warning);
                        MarkBatchItemConsumed(_batchKey);
                    }
                    else
                    {
                        MarkBatchItemConsumed(_batchKey);
                    }
                }
                catch
                {
                    Log($"Failed to process item: {JsonConvert.SerializeObject(queueItem.Item)}", level: LogLevel.Error);
                    MarkBatchItemConsumed(_batchKey);
                }
            }
        }

        Log($"Consumer completed.");
    }

    private void MarkBatchItemConsumed(Guid key, int count = 1)
    {
        if (_batchTotalConsumed.ContainsKey(key))
            _batchTotalConsumed[key] += count;
    }

    private void MarkBatchItemProduced(Guid key, int count = 1)
    {
        if (_batchTotalProcessed.ContainsKey(key))
            _batchTotalProcessed[key] += count;
    }

    private bool IsBatchCompleted(Guid key)
    {
        return !_batchTotalProcessed.ContainsKey(key) ||
               !_batchTotalConsumed.ContainsKey(key) ||
               _batchTotalProcessed[key] == _batchTotalConsumed[key];
    }

    private void InitBatch()
    {
        _batchKey = Guid.NewGuid();

        if (!_batchTotalProcessed.ContainsKey(_batchKey))
            _batchTotalProcessed.Add(_batchKey, 0);

        if (_batchTotalConsumed.ContainsKey(_batchKey))
            _batchTotalConsumed.Add(_batchKey, 0);

        _batchTotalProcessed[_batchKey] = 0;
        _batchTotalConsumed[_batchKey] = 0;
    }

    protected void EnqueueItem(TItem item)
    {
        MarkBatchItemProduced(_batchKey);
        _queue.Add(QueueItem<TItem>.New(item));
    }

    protected virtual void SaveStoreMappings<TEntity>(TEntity entity, Action updateEntityAction)
        where TEntity : BaseEntity, IStoreMappingSupported
    {
        SaveStoreMappings(entity, updateEntityAction, false);
    }

    protected virtual void SaveStoreMappings<TEntity>(TEntity entity, Action updateEntityAction, bool remove = false, bool limitedToStore = false)
        where TEntity : BaseEntity, IStoreMappingSupported
    {
        entity.LimitedToStores = false;
        updateEntityAction?.Invoke();
    }

    protected virtual void SaveCategorySlug<TEntity>(TEntity entity)
        where TEntity : BaseEntity, ICategoryLocalizedEntity, ISlugSupported
    {
        var seName = _urlRecordService.GetSeName(entity, 0, true, false);
        seName = _urlRecordService.ValidateSeName(entity, seName, entity.Name, true);
        _urlRecordService.SaveSlug(entity, seName, 0);
    }

    protected virtual void SaveManufacturerSlug<TEntity>(TEntity entity)
        where TEntity : BaseEntity, IManufacturerLocalizedEntity, ISlugSupported
    {
        var seName = _urlRecordService.GetSeName(entity, 0, true, false);
        seName = _urlRecordService.ValidateSeName(entity, seName, entity.Name, true);
        _urlRecordService.SaveSlug(entity, seName, 0);
    }

    protected virtual void SaveProductSlug<TEntity>(TEntity entity)
        where TEntity : BaseEntity, IProductLocalizedEntity, ISlugSupported
    {
        var seName = _urlRecordService.GetSeName(entity, 0, true, false);
        seName = _urlRecordService.ValidateSeName(entity, seName, entity.Name, true);
        _urlRecordService.SaveSlug(entity, seName, 0);
    }

    protected TType GetSetting<TType>(string settingName, TType defaultValue = default)
    {
        var setting = _settingService.GetSetting(settingName, RunningOnStoreId)?.Value;
        if (setting == null)
            return defaultValue;

        return _objectConverter.ToType<TType>(setting);
    }

    protected void Debug(string msg)
    {
        Log(msg, level: LogLevel.Debug);
    }

    protected void Error(string msg, Exception ex)
    {
        Log(msg, ex, LogLevel.Error);
    }

    protected void JobOutput(string message)
    {
        var taskName = StoreContextualName.Replace($"{_namespace}.", "");
        var queueCount = !_disposed ? _queue.Count : 0;

        var log = $"{taskName} ({queueCount} items): {message}";

        _jobLogger.LogInformation(log);
    }

    protected void Log(string msg, Exception ex = null, LogLevel level = LogLevel.Information)
    {
        var taskName = StoreContextualName.Replace($"{_namespace}.", "");
        var queueCount = !_disposed ? _queue.Count : 0;

        var log = $"{taskName} ({queueCount} items): {msg}";

        switch (level)
        {
            case LogLevel.Trace:
            case LogLevel.Debug:
                if (_enableLog)
                {
                    _logger.InsertLog(Core.Domain.Logging.LogLevel.Debug, log);
                    _jobLogger.LogDebug(log);
                }
                break;

            case LogLevel.Information:
                if (_enableLog)
                {
                    _logger.Information(log);
                    _jobLogger.LogInformation(log);
                }
                break;

            case LogLevel.Warning:
                if (_enableLog)
                {
                    _logger.Warning(log, ex);
                    _jobLogger.LogWarning(log);
                }
                break;

            case LogLevel.Critical:
            case LogLevel.Error:
                _logger.Error(log, ex);
                _jobLogger.LogError(log);
                break;

            case LogLevel.None:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
}
public class QueueItem<T>
{
    private const int MaxRetryCount = 3;

    public T Item { get; set; }
    public bool Requeue { get; set; }
    public int RetryCount { get; set; }

    public static QueueItem<T> New(T item)
    {
        return new QueueItem<T>
        {
            Item = item
        };
    }

    public static QueueItem<T> Default()
    {
        return New(default);
    }

    public bool AllowRequeue => Requeue && RetryCount < MaxRetryCount;
}
