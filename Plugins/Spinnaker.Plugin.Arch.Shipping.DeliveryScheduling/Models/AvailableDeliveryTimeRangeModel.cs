using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public record AvailableDeliveryTimeRangeModel : BaseNopEntityModel
    {
        public AvailableDeliveryTimeRangeModel()
        {
            AvailableDeliveryDateTimeRangeModel = new List<SelectListItem>();
        }

        [Display(Name = "Time")]
        public string Time { get; set; }

        public int StoreId { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Deleted")]
        public bool Deleted { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public IList<SelectListItem> AvailableDeliveryDateTimeRangeModel { get; set; }

    }
}
