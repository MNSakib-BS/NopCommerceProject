using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Widgets.OurTeam.Domain;
using Nop.Plugin.Widgets.OurTeam.Factories;
using Nop.Plugin.Widgets.OurTeam.Models;
using Nop.Plugin.Widgets.OurTeam.Services;
using Nop.Services.Media;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Widgets.OurTeam.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.ADMIN)]
    //[AutoValidateAntiforgeryToken]
    public class EmployeesController : BasePluginController
    {
        private readonly IEmployeesService _employeeService;
        private readonly IEmployeesModelFactory _employeeModelFactory;
        protected readonly IPictureService _pictureService;

        public EmployeesController(IEmployeesService employeeService,
            IEmployeesModelFactory employeeModelFactory,
            IPictureService pictureService)
        {
            _employeeModelFactory = employeeModelFactory;   
            _employeeService = employeeService;
            _pictureService = pictureService;
        }
        public async Task<IActionResult> List()
        {
            var searchModel = await _employeeModelFactory.PrepareEmployeeSearchModelAsyc(new EmployeeSearchModel());
            
            return View("~/Plugins/Widgets.OurTeam/Views/Employees/List.cshtml", searchModel);
        }
        [HttpPost]
        public async Task<IActionResult> List(EmployeeSearchModel searchModel)
        {
            var model = await _employeeModelFactory.PrepareEmployeeListModelAsyc(searchModel);

            return Json(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = await _employeeModelFactory.PrepareEmployeeModelAsyc(new EmployeeModel(), null);

            return View("~/Plugins/Widgets.OurTeam/Views/Employees/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Create(EmployeeModel model , bool continueEditing)
        {
            model.EmployeeStatusStr = ((EmployeeStatus)model.EmployeeStatusId).ToString();
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var employee = new Employees
                {
                    Designation = model.Designation,
                    EmployeeStatusId = model.EmployeeStatusId,
                    IsMVP = model.IsMVP,
                    IsNopCommerceCertified = model.IsNopCommerceCertified,
                    Name = model.Name,
                    PictureId = model.PictureId
                    
                };

                await _employeeService.InsertEmployeeAsync(employee);
                await UpdatePictureSeoNamesAsync(employee);

                return continueEditing ? RedirectToAction("Edit" , new {id = employee.Id}): RedirectToAction("List");
            }

            
            model = await _employeeModelFactory.PrepareEmployeeModelAsyc(model, null);
            return View("~/Plugins/Widgets.OurTeam/Views/Employees/Create.cshtml", model);
        }
        protected virtual async Task UpdatePictureSeoNamesAsync(Employees employee)
        {
            var picture = await _pictureService.GetPictureByIdAsync(employee.PictureId);
            if (picture != null)
                await _pictureService.SetSeoFilenameAsync(picture.Id, await _pictureService.GetPictureSeNameAsync(employee.Name));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee != null)
            {
                var model = await _employeeModelFactory.PrepareEmployeeModelAsyc(null, employee);
                return View("~/Plugins/Widgets.OurTeam/Views/Employees/Edit.cshtml", model);

            }

            return RedirectToAction("List");

        }


        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public async Task<IActionResult> Edit(EmployeeModel model, bool continueEditing)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(model.Id);
            if (employee == null)
                return RedirectToAction("List");
            model.EmployeeStatusStr = ((EmployeeStatus)model.EmployeeStatusId).ToString();
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var prevPictureId = employee.PictureId;

                employee.Designation = model.Designation;
                employee.EmployeeStatusId = model.EmployeeStatusId;
                employee.IsMVP = model.IsMVP;
                employee.IsNopCommerceCertified = model.IsNopCommerceCertified;
                employee.Name = model.Name;

             
                await _employeeService.UpdateEmployeeAsync(employee);
                if (prevPictureId > 0 && prevPictureId != employee.PictureId)
                {
                    var prevPicture = await _pictureService.GetPictureByIdAsync(prevPictureId);
                    if (prevPicture != null)
                        await _pictureService.DeletePictureAsync(prevPicture);
                }

                // Update picture SEO filename
                await UpdatePictureSeoNamesAsync(employee);
                return RedirectToAction("List");
            }

            
            model = await _employeeModelFactory.PrepareEmployeeModelAsyc(model, employee);

            return continueEditing ? RedirectToAction("Edit", new { id = employee.Id }) : View("~/Plugins/Widgets.OurTeam/Views/Employees/Edit.cshtml", model);

        }

        [HttpPost]
        public async Task<IActionResult> Delete(EmployeeModel model)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(model.Id);
            if (employee == null)
                return RedirectToAction("List");

            await _employeeService.DeleteEmployeeAsync(employee);
            return RedirectToAction("List");
        }

    }
}
