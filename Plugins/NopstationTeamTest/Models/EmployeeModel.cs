using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.NopstationTeamTest.Models
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
//Title = T("Admin.Orders.Fields.PaymentStatus").Text,