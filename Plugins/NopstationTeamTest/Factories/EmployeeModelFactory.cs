using DocumentFormat.OpenXml.Wordprocessing;
using Nop.Plugin.Misc.NopstationTeamTest.Domain;
using Nop.Plugin.Misc.NopstationTeamTest.Models;
using Nop.Plugin.Misc.NopstationTeamTest.Services;
using Nop.Services;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.NopstationTeamTest.Factories
{
    public class EmployeeModelFactory : IEmployeeModelFactory
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;


        public EmployeeModelFactory(IEmployeeService employeeService, 
            ILocalizationService localizationService ,IPictureService pictureService)
        {
            _employeeService = employeeService;
            _localizationService = localizationService;
            _pictureService = pictureService;
        }

        public async Task<EmployeeListModel> PrepareEmployeeListModelAsyc(EmployeeSearchModel searchModel)
        {
            ArgumentNullException.ThrowIfNull(nameof(searchModel));
            var employees = await _employeeService.SearchEmployeesAsync(searchModel.Name, searchModel.EmployeeStatusId,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);
          /*  var model = await new EmployeeListModel().PrepareToGridAsync(searchModel, employees, () =>
            {
                return employees.SelectAwait(async employee =>
                {
                    var employeeModel = new EmployeeModel()
                    {
                        Designation = employee.Designation,
                        EmployeeStatusId = employee.EmployeeStatusId,
                        Id = employee.EmployeeStatusId,
                        IsMVP = employee.IsMVP,
                        IsNopCommerceCertified = employee.IsNopCommerceCertified,
                        Name = employee.Name,
                        EmployeeStatusStr = await _localizationService.GetLocalizedEnumAsync(employee.EmployeeStatus)

                    };
                    //replace by function
                    return await PrepareEmployeeModelAsyc(null, employee, true);
                });
            });*/
            var model = await new EmployeeListModel().PrepareToGridAsync(searchModel, employees, () =>
                employees.SelectAwait(async employee => await PrepareEmployeeModelAsyc(null, employee, true))
            );

            return model;
        }
        public async Task<EmployeeModel> PrepareEmployeeModelAsyc(EmployeeModel model, Employee employee, bool excludeProperties = false)
        {
            if (model == null)
            {
                model = new EmployeeModel();
            }

            if (employee != null)
            {
                //TODO: AUtomapper
                model.Designation = employee.Designation;
                model.EmployeeStatusId = employee.EmployeeStatusId;
                model.Id = employee.Id;
                model.IsMVP = employee.IsMVP;
                model.IsNopCommerceCertified = employee.IsNopCommerceCertified;
                model.Name = employee.Name;
                model.EmployeeStatusStr = await _localizationService.GetLocalizedEnumAsync(employee.EmployeeStatus);
                model.PictureId = employee.PictureId;


                var picture = await _pictureService.GetPictureByIdAsync(employee.PictureId);
                (model.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(picture, 75);
            }
            
            if (!excludeProperties)
            {
                model.AvailableEmployeeStatusOptions = (await EmployeeStatus.Active.ToSelectListAsync()).ToList();
            }
            return model;
        }


        public async Task<EmployeeSearchModel> PrepareEmployeeSearchModelAsyc(EmployeeSearchModel searchModel)
        {
            ArgumentNullException.ThrowIfNull(nameof(searchModel));
            searchModel.AvailableEmployeeStatusOptions =(await EmployeeStatus.Active.ToSelectListAsync()).ToList();
            searchModel.AvailableEmployeeStatusOptions.Insert(0,
                new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = "All",
                    Value = "0"

                });

            searchModel.SetGridPageSize();
            return searchModel;    
        }
    } 
}
 