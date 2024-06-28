using System;
using Microsoft.AspNetCore.Mvc;
using Arch.Core.Services.Payments;
using Nop.Core;
using Nop.Services.Customers;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;

namespace Arch.Core.Areas.Admin.Controllers
{
    public partial class ArchCustomerWalletController : BaseAdminController
    {
        #region Fields
        
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerWalletService _customerWalletService;

        #endregion

        #region Ctor

        public ArchCustomerWalletController(ICustomerModelFactory customerModelFactory,
            ICustomerService customerService,
            IPermissionService permissionService,
            ICustomerWalletService customerWalletService)
        {
            _customerModelFactory = customerModelFactory;
            _customerService = customerService;
            _permissionService = permissionService;
            _customerWalletService = customerWalletService;
        }

        #endregion

        [HttpPost]
        public virtual IActionResult CustomerWalletList(CustomerWalletSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            var customer = _customerService.GetCustomerById(searchModel.CustomerId);

            //prepare model
            var model = _customerModelFactory.PrepareCustomerWalletListModel(searchModel, customer);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult CustomerWalletTransactionList(CustomerWalletTransactionSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            var customerWallet = _customerWalletService.GetCustomerWalletById(searchModel.CustomerWalletId);

            //prepare model
            var model = _customerModelFactory.PrepareCustomerWalletTransactionListModel(searchModel, customerWallet);

            return Json(model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer wallet with the specified id
            var customerWallet = _customerWalletService.GetCustomerWalletById(id);
            if (customerWallet == null)
                return RedirectToAction("List", "Customer");

            //prepare model
            var model = _customerModelFactory.PrepareCustomerWalletModel(null, customerWallet);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ClaimCashback(CustomerWalletTransactionRefundModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Json(new { RedirectUrl = "/Admin/Customer/List" });

            var success = _customerWalletService.ClaimCashback(model.Id,
                Convert.ToInt64(model.LastModifiedTicks),
                out var customerWalletId);

            if (customerWalletId.HasValue)
                return Json(new { RedirectUrl = "/Admin/CustomerWallet/Edit/" + customerWalletId.Value });

            return Json(new { RedirectUrl = "/Admin/Customer/List" });
        }

        [HttpPost]
        public virtual IActionResult CreditToWallet(CustomerWalletTransactionRefundModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return Json(new { RedirectUrl = "/Admin/Customer/List" });

            var success = _customerWalletService.VoidCashback(model.Id,
                Convert.ToInt64(model.LastModifiedTicks),
                out var customerWalletId);

            if (customerWalletId.HasValue)
                return Json(new { RedirectUrl = "/Admin/CustomerWallet/Edit/" + customerWalletId.Value });

            return Json(new { RedirectUrl = "/Admin/Customer/List" });
        }
    }
}