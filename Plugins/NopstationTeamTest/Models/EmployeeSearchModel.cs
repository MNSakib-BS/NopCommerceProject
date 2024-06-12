using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace Nop.Plugin.Misc.NopstationTeamTest.Models
{
    public record EmployeeSearchModel : BaseSearchModel
    {
        public EmployeeSearchModel()
        {
            AvailableEmployeeStatusOptions = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Misc.Employee.List.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.Misc.Employee.List.EmployeeStatusId")]
        public int EmployeeStatusId { get; set; }

        public IList<SelectListItem> AvailableEmployeeStatusOptions { get; set; }
    }
}
 