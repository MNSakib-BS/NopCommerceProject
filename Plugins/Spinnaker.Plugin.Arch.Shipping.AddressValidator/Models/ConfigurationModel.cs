using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Spinnaker.Plugin.Arch.Shipping.AddressValidator.Models;
public record ConfigurationModel : BaseNopModel
{
    public int ActiveStoreScopeConfiguration { get; set; }

    [NopResourceDisplayName("Spinnaker.Plugins.Arch.Addressvalidator.CountryRestriction")]
    public string CountryRestriction { get; set; }
    public bool CountryRestriction_OverrideForStore { get; set; }

    public IList<SelectListItem> AvailableCountries { get; set; }

    [DisplayName("Selected Country Codes")]
    [NopResourceDisplayName("Spinnaker.Plugins.Arch.Addressvalidator.SelectedCountryCodes")]
    public IList<string> SelectedCountryCodes { get; set; }
}
