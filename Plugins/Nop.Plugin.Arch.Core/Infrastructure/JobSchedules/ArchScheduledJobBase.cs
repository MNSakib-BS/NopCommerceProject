using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchServiceReference;
using Microsoft.Extensions.Logging;
using Nop.Core.Infrastructure;
using Nop.Core;
using Nop.Plugin.Arch.Core.Services.Helpers;
using Nop.Services.Configuration;
using Nop.Services.Seo;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
public abstract class ArchScheduledJobBase<T> : ScheduledJobBase<T>
{
    private const string _dateFormat = "u";

    private ECommerceBOClient _archClient { get; set; }
    protected readonly ArchApiManager _archApiManager;

    public ECommerceBOClient ArchClient
    {
        get
        {
            if (_archClient.Endpoint.Address.Uri != new Uri(_archSettings.ApiEndpointAddress))
                throw new ArgumentException("Arch API Endpoint not set");
            return _archClient;
        }
    }

    protected ArchScheduledJobBase(ISettingService settingService,
        IStoreService storeService,
        IStoreContext storeContext,
        IStoreMappingService storeMappingService,
        IUrlRecordService urlRecordService,
        ILogger logger,
        IObjectConverter objectConverter,
        ILogger<ScheduledJobBase<object>> jobLogger)
        : base(settingService,
            storeService,
            storeContext,
            storeMappingService,
            urlRecordService,
            logger,
            objectConverter,
            jobLogger)
    {
        _archApiManager = EngineContext.Current.Resolve<ArchApiManager>();
    }

    public override void SetStoreId(int storeId)
    {
        base.SetStoreId(storeId);

        EnsureSettings();
    }

    protected override void OnExecuting()
    {
        base.OnExecuting();

        EnsureSettings();
    }

    protected void EnsureSettings()
    {
        if (string.IsNullOrWhiteSpace(_archSettings.ApiEndpointAddress))
            return;

        //if (_archSettings.ApiEndpointAddress.StartsWith("https"))// ensure security mode is set when url is SSL
        //{
        //    var binding = _archClient.Endpoint.Binding as BasicHttpBinding;
        //    binding.Security.Mode = BasicHttpSecurityMode.Transport;
        //}

        //_archClient.Endpoint.Address = new EndpointAddress(_archSettings.ApiEndpointAddress);

        //var sendTimeSpanParts = TimeSpanParts.New(_archSettings.SendTimeout);
        //_archClient.Endpoint.Binding.SendTimeout = new TimeSpan(sendTimeSpanParts.Hours, sendTimeSpanParts.Minutes, sendTimeSpanParts.Seconds);

        //var receiveTimeSpanParts = TimeSpanParts.New(_archSettings.ReceiveTimeout);
        //_archClient.Endpoint.Binding.ReceiveTimeout = new TimeSpan(receiveTimeSpanParts.Hours, receiveTimeSpanParts.Minutes, receiveTimeSpanParts.Seconds);
        _archClient = _archApiManager.GetECommerceBOClient(RunningOnStoreId);

        if (_archClient == null)
            return;

        //DisableCertValidationAuth();
    }

    protected DateTime GetLastUpdate(string settingName)
    {
        var lastUpdate = _archSettings.Epoch;

        var lastUpdateSetting = _settingService.GetSetting(settingName, RunningOnStoreId);
        if (lastUpdateSetting != null)
        {
            lastUpdate = DateTime.ParseExact(lastUpdateSetting.Value, _dateFormat, null);
        }
        Log($"Requesting data from {lastUpdate.ToString("yyyy-MM-dd")}", null, LogLevel.Debug);
        return lastUpdate;
    }

    protected void SetLastUpdate(string settingName)
    {
        _settingService.SetSetting(settingName, DateTime.Now.ToString(_dateFormat), RunningOnStoreId);
    }


}
