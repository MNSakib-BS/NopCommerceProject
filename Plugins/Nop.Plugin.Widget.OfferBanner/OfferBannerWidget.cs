using Nop.Core;
using Nop.Plugin.Widget.OfferBanner.Components;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widget.OfferBanner
{
    public class OfferBannerWidget : BasePlugin, IWidgetPlugin
    {
        private IWebHelper _webHelper;

        public OfferBannerWidget(IWebHelper webHelper)
        {
            _webHelper = webHelper;
        }
        public bool HideInWidgetList => false;

        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(OfferBannerViewComponent);
        }

        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.ProductDetailsTop, PublicWidgetZones.ProductDetailsBeforeCollateral });

        }
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/OfferBanner/Configure";
        }
    }
}
