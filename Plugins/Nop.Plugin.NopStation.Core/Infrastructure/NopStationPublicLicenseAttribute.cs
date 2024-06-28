using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Data;
using Nop.Plugin.NopStation.Core.Services;

namespace Nop.Plugin.NopStation.Core.Infrastructure
{
    public partial class NopStationPublicLicenseAttribute : TypeFilterAttribute
    {
        #region Ctor

        public NopStationPublicLicenseAttribute() : base(typeof(NopStationPublicLicenseFilter))
        {
        }

        #endregion

        #region Nested filter

        private class NopStationPublicLicenseFilter : IAuthorizationFilter
        {
            #region Fields

            private readonly INopStationLicenseService _nopStationLicenseService;

            #endregion

            #region Ctor

            public NopStationPublicLicenseFilter(INopStationLicenseService nopStationLicenseService)
            {
                _nopStationLicenseService = nopStationLicenseService;
            }

            #endregion

            #region Methods

            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                    throw new ArgumentNullException(nameof(filterContext));

                if (!DataSettingsManager.DatabaseIsInstalled)//_databaseIsInstalled
                    return;

                if(!_nopStationLicenseService.IsLicensed())
                    filterContext.Result = new RedirectToActionResult("Index", "Home", null);
            }

            #endregion
        }

        #endregion
    }
}