using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Widgets.OurTeam.Models
{
    public record EmployeeModel : BaseNopEntityModel
    {
        public EmployeeModel()
        {
            AvailableEmployeeStatusOptions = new List<SelectListItem>();
        }
        [NopResourceDisplayName("Admin.Misc.Employee.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Misc.Employee.Fields.Designation")]
        public string Designation { get; set; }

        [NopResourceDisplayName("Admin.Misc.Employee.Fields.IsMVP")]
        public bool IsMVP { get; set; }

        [NopResourceDisplayName("Admin.Misc.Employee.Fields.IsNopCommerceCertified")]
        public bool IsNopCommerceCertified { get; set; }

        [NopResourceDisplayName("Admin.Misc.Employee.Fields.EmployeeStatus")]
        public int EmployeeStatusId { get; set; }
        [NopResourceDisplayName("Admin.Misc.Employee.Fields.EmployeeStatus")]
        public string EmployeeStatusStr { get; set; }

        public IList<SelectListItem> AvailableEmployeeStatusOptions { get; set; }

        public string PictureThumbnailUrl { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Misc.Developer.Fields.Picture")]
        public int PictureId { get; set; }




    }
}
