using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Catalog;
using Nop.Services.Security;
using Nop.Web.Controllers;
using Nop.Web.Factories;
using Nop.Web.Framework.Mvc.Filters;

namespace Arch.Core.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class RecommendedListController : BasePublicController
    {
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly IRecommendedListService _recommendedListService;
        private readonly IProductModelFactory _productModelFactory;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;

        public RecommendedListController(
            IPermissionService permissionService,
            IProductService productService,
            IWorkContext workContext,
            IRecommendedListService recommendedListService,
            IProductModelFactory productModelFactory,
            IShoppingCartModelFactory shoppingCartModelFactory)
        {
            _permissionService = permissionService;
            _productService = productService;
            _workContext = workContext;
            _recommendedListService = recommendedListService;
            _productModelFactory = productModelFactory;
            _shoppingCartModelFactory = shoppingCartModelFactory;
        }

        [HttpsRequirement]
        public virtual IActionResult RecommendedList(int recommendedListId, WishlistModelPagingFilteringModel command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return RedirectToRoute("Homepage");

            var customer = _workContext.CurrentCustomer;
            if (customer == null)
                return RedirectToRoute("Homepage");

            var recommendedList = _recommendedListService.GetRecommendedListById(recommendedListId);
            if (recommendedList == null)
                recommendedList = _recommendedListService.GetAllRecommendedLists().FirstOrDefault();

            if (recommendedList == null)
            {
                return View(new RecommendedListModel
                {
                    SelectedRecommendedListId = 0,
                    Name = string.Empty
                });
            }

            var model = new RecommendedListModel
            {
                SelectedRecommendedListId = recommendedList.Id,
                Name = recommendedList.Name
            };

            _shoppingCartModelFactory.PrepareRecommendedListModel(model, command);

            return View(model);
        }
    }
}
