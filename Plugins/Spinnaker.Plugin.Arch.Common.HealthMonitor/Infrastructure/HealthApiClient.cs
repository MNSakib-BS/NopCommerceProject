using Nop.Web.Framework;

using Nop.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Infrastructure
{
    public class HealthApiClient : WebApiBase
    {
        protected readonly ISettingService _settingService;

        public HealthApiClient(ISettingService settingService)
        {
            //this.BaseUrl
            _settingService = settingService;
        }
    }
}
