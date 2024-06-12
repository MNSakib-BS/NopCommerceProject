using DocumentFormat.OpenXml.Wordprocessing;
using Nop.Plugin.Widgets.OurTeam.Domain;
using Nop.Plugin.Widgets.OurTeam.Models;
using Nop.Plugin.Widgets.OurTeam.Services;
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

namespace Nop.Plugin.Widgets.OurTeam.Factories
{
    public class EmployeesModelFactory : IEmployeesModelFactory
    {
        private readonly IEmployeesService _employeeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;


        public EmployeesModelFactory(IEmployeesService employeeService, 
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

            var model = await new EmployeeListModel().PrepareToGridAsync(searchModel, employees, () =>
                employees.SelectAwait(async employee => await PrepareEmployeeModelAsyc(null, employee, true))
            );

            return model;
        }
        public async Task<EmployeeModel> PrepareEmployeeModelAsyc(EmployeeModel model, Employees employee, bool excludeProperties = false)
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
        public async Task<List<EmployeeModel>> PrepareAllEmployeesAsync()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();

            var sortedEmployees = employees.OrderBy(employee => GetDesignationPriority(employee.Designation));

            var employeeModels = new List<EmployeeModel>();
            foreach (var employee in sortedEmployees)
            {
                var employeeModel = new EmployeeModel
                {
                    Designation = employee.Designation,
                    EmployeeStatusId = employee.EmployeeStatusId,
                    Id = employee.Id,
                    IsMVP = employee.IsMVP,
                    IsNopCommerceCertified = employee.IsNopCommerceCertified,
                    Name = employee.Name,
                    EmployeeStatusStr = await _localizationService.GetLocalizedEnumAsync(employee.EmployeeStatus),
                    PictureId = employee.PictureId
                };

                var picture = await _pictureService.GetPictureByIdAsync(employee.PictureId);
                (employeeModel.PictureThumbnailUrl, _) = await _pictureService.GetPictureUrlAsync(picture,150);

                employeeModels.Add(employeeModel);
            }

            return employeeModels;
        }

        private int GetDesignationPriority(string designation)
        {
            switch (designation)
            {
                case "Head of nopStation":
                    return 1;
                case "Project Manager":
                    return 2;
                case "Principal Engineer":
                    return 3;
                case "Lead Engineer":
                    return 4;
                case "Senior Software Engineer [Team Lead]":
                    return 6;
                case "Senior UI Engineer [Team Lead]":
                    return 7;
                case "Senior Software Engineer":
                    return 8;
                case "Senior Business Analyst":
                    return 9;
                case "Software Engineer":
                    return 10;
                case "Business Analyst":
                    return 11;
                case "Associate Software Engineer":
                    return 12;
                case "Associate UI Engineer":
                    return 13;
                case "Associate SQA Engineer":
                    return 14;
                default:
                    return int.MaxValue;
            }
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
 