using Nop.Web.Areas.Admin.Models.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.Utility.Models
{
    public class StoreLinkModel
    {
        public int StoreID { get; set; }
        public List<StoreLookupModel> Stores { get; set; }
        public int LinkedStoreID { get; set; }
    }

    public class StoreLookupModel
    {
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public string Url { get; set; }
        public string StoreTypeThumbnailUrl { get; set; }
        public int StoreType { get; set; }
        public bool UseStoreTypeImage { get; internal set; }
    }

}
