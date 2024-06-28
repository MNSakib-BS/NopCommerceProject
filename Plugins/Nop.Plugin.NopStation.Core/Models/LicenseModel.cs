using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.Core.Models
{
    public partial record LicenseModel : BaseNopModel, ISettingsModel
    {
        [NopResourceDisplayName("Admin.NopStation.Core.License.LicenseString")]
        public string LicenseString { get; set; }
        public bool LicenseString_OverrideForStore { get; set; }

        public int ActiveStoreScopeConfiguration { get; set; }
    }
}
