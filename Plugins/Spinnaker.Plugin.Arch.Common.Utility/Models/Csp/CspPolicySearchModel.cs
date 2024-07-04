using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.Utility.Models.Csp
{
    public  record CspPolicySearchModel : BaseSearchModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}
    }
}
