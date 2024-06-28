using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Data;
using Nop.Plugin.NopStation.Core.Services;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public partial class NopStationLicenseAttribute : TypeFilterAttribute
    {
        #region Ctor

        public NopStationLicenseAttribute() : base(typeof(NopStationLicenseFilter))
        {
        }

        #endregion

        #region Nested filter

        private class NopStationLicenseFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly INopStationLicenseService _nopStationLicenseService;

            #endregion

            #region Ctor

            public NopStationLicenseFilter(INopStationLicenseService nopStationLicenseService)
            {
                _nopStationLicenseService = nopStationLicenseService;
            }

            #endregion

            #region Methods

            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                if (!DataSettingsManager.DatabaseIsInstalled)
                    return;

                if(!_nopStationLicenseService.IsLicensed())
                    filterContext.Result = new RedirectToActionResult("License", "NopStationCore", null);
            }

            #endregion
        }

        #endregion
    }
}