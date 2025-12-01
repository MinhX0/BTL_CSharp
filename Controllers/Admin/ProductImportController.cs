using System.Threading.Tasks;
using backend.Services.Store;
using backend.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Admin
{
    [Route("Admin/Products/Import")]
    public class ProductImportController : Controller
    {
        private readonly IProductService _productService;

        public ProductImportController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            var vm = new ProductImportViewModel
            {
                Products = products.ToList()
            };
            return View("~/Views/Admin/ProductImport.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(ProductImportViewModel model)
        {
            var products = await _productService.GetAllAsync();
            model.Products = products.ToList();

            if (!model.SelectedProductId.HasValue || model.QuantityToAdd <= 0)
            {
                model.Error = "Please select a product and enter a positive quantity.";
                return View("~/Views/Admin/ProductImport.cshtml", model);
            }

            var ok = await _productService.AddStockAsync(model.SelectedProductId.Value, model.QuantityToAdd);
            if (ok)
            {
                model.Message = "Stock updated successfully.";
            }
            else
            {
                model.Error = "Failed to update stock. Product may not exist.";
            }
            // reset quantity input
            model.QuantityToAdd = 0;
            return View("~/Views/Admin/ProductImport.cshtml", model);
        }
    }
}