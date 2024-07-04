using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.Utility.Models.Csp
{
    public record CspReportListItemModel : BaseNopEntityModel
    {
        public string DocumentUri { get; set; }

        public string Referrer { get; set; }

        public string ViolatedDirective { get; set; }

        public string EffectiveDirective { get; set; }

        public string OriginalPolicy { get; set; }

        public string BlockedUri { get; set; }

        public int StatusCode { get; set; }
        public DateTime LoggedAt { get; internal set; }
    }
}
