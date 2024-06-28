using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchServiceReference;
using Nop.Core.Domain.Customers;
using Nop.Core;
using Nop.Plugin.Arch.Core.Services.Helpers;
using Nop.Plugin.Arch.Core.Services.Orders;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Plugin.Arch.Core.Services.Customers;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Services.Stores;

namespace Nop.Plugin.Arch.Core.Services.ArchAPI;
public class ArchArchServicesApi : ArchArchApiBase
{
    public ArchArchServicesApi(ISettingService settingService,
        ECommerceBOClient client,
        CustomerSettings customerSettings,
        ILogger logger,
        IObjectConverter objectConverter,
        ICustomerAdditionalService customerAdditionalService,
        CustomerSettingsAdditional customerSettingsAdditional,
        IStoreAdditionalService storeAdditionalService) : base(settingService, client, customerSettings, logger, objectConverter, customerAdditionalService, customerSettingsAdditional, storeAdditionalService)
    {
    }
}
