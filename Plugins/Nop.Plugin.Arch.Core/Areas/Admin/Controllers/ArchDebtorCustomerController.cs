using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Arch.Core.Areas.Admin.Controllers
{
    public partial class ArchDebtorCustomerController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor

        public ArchDebtorCustomerController(ICustomerModelFactory customerModelFactory,
            IPermissionService permissionService)
        {
            _customerModelFactory = customerModelFactory;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _customerModelFactory.PrepareDebtorCustomerSearchModel(new DebtorCustomerSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(DebtorCustomerSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageCustomers))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _customerModelFactory.PrepareCustomerListModelAsync(searchModel);

            return Json(model);
        }

        #endregion
    }
}