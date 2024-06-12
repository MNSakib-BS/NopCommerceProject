using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Widgets.OurTeam.Models
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
