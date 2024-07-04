using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.Utility.Areas.Admin.Models.Catalog
{
    public record ArchProductElementModel : BaseNopModel, IStoreMappingSupportedModel
    {
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
        public string ProductName { get; internal set; }
        public string ProductCode { get; internal set; }
        public decimal Price { get; internal set; }
    }
}
