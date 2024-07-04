using Nop.Web.Framework.Models;
using Spinnaker.Plugin.Arch.Common.Utility.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.Utility.Areas.Admin.Models.Catalog
{
    public record ArchProductElementListModel : BasePagedListModel<ArchProductElementModel>
    {
    }
}
