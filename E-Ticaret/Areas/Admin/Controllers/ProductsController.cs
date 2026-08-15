using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using ETicaretWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;

        public ProductsController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();

            var viewModelList = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                FormattedPrice = p.Price.ToString("C"),
                CategoryName = p.Category?.Name ?? "Belirtilmemiş",
                SellerName = p.User?.FirstName ?? "Bilinmeyen Satıcı",
                ImageUrl = string.IsNullOrEmpty(p.ImageUrl) ? "https://via.placeholder.com/100" : p.ImageUrl
            }).ToList();

            return View(viewModelList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product != null)
            {
                _productRepository.Delete(product);
                await _productRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ürün sistemden kalıcı olarak silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}