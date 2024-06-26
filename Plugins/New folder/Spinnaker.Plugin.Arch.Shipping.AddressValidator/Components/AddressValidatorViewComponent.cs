using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using Spinnaker.Plugin.Arch.Shipping.AddressValidator.Models;

namespace Spinnaker.Plugin.Arch.Shipping.AddressValidator.Components;
[ViewComponent(Name = "AddressValidator")]
public class AddressValidatorViewComponent : NopViewComponent
{
    //private readonly GoogleApiService _googleApi;
    private readonly ISettingService _settingService;
    private readonly IStoreContext _storeContext;

    public AddressValidatorViewComponent(ISettingService settingService , IStoreContext storeContext)
    {
        //_googleApi = EngineContext.Current.Resolve<GoogleApiService>();
        _storeContext = storeContext;
        _settingService = settingService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var storeScope =await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var addressValidatorSettings =  _settingService.LoadSetting<AddressValidatorSettings>(storeScope);
        var countryCode = !string.IsNullOrEmpty(addressValidatorSettings.CountryRestriction) ? addressValidatorSettings.CountryRestriction : "ZA";
        var model = new AddressValidatorModel {/* GoogleApiKey = _googleApi.GetAPIKey(), CountryCode = countryCode*/ };

        return View("~/Plugins/Arch.Addressvalidator/Views/AddressValidator.cshtml", model);
    }
}
