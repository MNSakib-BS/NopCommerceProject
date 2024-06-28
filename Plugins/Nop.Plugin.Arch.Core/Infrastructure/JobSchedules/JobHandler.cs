using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Api;
using Nop.Services.Configuration;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Infrastructure.JobSchedules;
public class JobHandler<TJob, T> where TJob : ScheduledJobBase<T>
{
    protected readonly IRepository<Store> _storeRepository;
    private readonly IJobManager _jobManager;
    protected readonly ISettingService _settingService;
    public JobHandler(IStoreService storeService,
        IRepository<Store> storeRepository,
        IJobManager jobManager
        )
    {
        _storeRepository = storeRepository;
        _jobManager = jobManager;
        _settingService = EngineContext.Current.Resolve<ISettingService>();
    }

    public void ExecuteJob()
    {
        var storeData = _storeRepository.Table.Select(s => new StoreReader { StoreID = s.Id, IsGlobalStore = s.IsGlobalStore }).ToList();

        foreach (var store in storeData)
        {
            try
            {
                var _archSettings = _settingService.LoadSetting<ArchApiSettings>(store.StoreID);

                if (_archSettings.AutoSyncEnabled)
                    _jobManager.Start<TJob>(x => x.Execute(store.StoreID, storeData.Count() > 1));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class StoreReader
    {
        public int StoreID { get; set; }
        public bool IsGlobalStore { get; set; }
    }
}
