using Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Resources;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;


namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Areas.Admin.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public ConfigurationModel()
        {
            WidgetZones = new List<int>();
            TimeRangeSearchModel = new TimeRangeSearchModel();
            ShippingMethodAvailableDateCapacity = new ShippingMethodAvailableDateCapacity();
            ShippingMethods = new List<SelectListItem>();
        }

        public int StoreId { get; set; }

        public int StoreCount { get; set; }

        public ShippingMethodAvailableDateCapacity ShippingMethodAvailableDateCapacity { get; set; }

        public TimeRangeSearchModel TimeRangeSearchModel { get; set; }

        [NopResourceDisplayName(ConfigurationResources.widgetZones)]
        public IList<int> WidgetZones { get; set; }

        public IList<SelectListItem> AvailableWidgetZones { get; set; }

        public IList<SelectListItem> ShippingMethods { get; set; }

        public string ShippingMethodId { get; set; }

        public int HourOffset { get; set; }

        public int DayOffset { get; set; }

    }
}
