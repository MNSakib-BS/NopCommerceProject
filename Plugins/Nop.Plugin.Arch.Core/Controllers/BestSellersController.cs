using Microsoft.AspNetCore.Mvc;
using Nop.Services.Orders;
using Nop.Core;
using Nop.Web.Factories;
using Nop.Web.Controllers;

namespace Nop.Plugin.Arch.Core.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class BestSellersController : BasePublicController
    {
        private readonly IStoreContext _storeContext;
        private readonly IOrderReportService _orderReportService;
        private readonly ICatalogModelFactory _catalogModelFactory;
        public BestSellersController(ICatalogModelFactory catalogModelFactory, IStoreContext storeContext)
        {

            _storeContext = storeContext;
            _catalogModelFactory = catalogModelFactory;

        }
        public IActionResult Index(CatalogPagingFilteringModel command)
        {
            var model = _catalogModelFactory.GetBestSellers(_storeContext.CurrentStore.Id, command);

            return View(model);
        }
    }
}
