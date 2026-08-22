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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IRepository<Product> productRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync(
            p => p.Category,
            p => p.User
            );

            var viewModelList = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                FormattedPrice = p.Price.ToString("C"),
                CategoryName = p.Category?.Name ?? "Belirtilmemiş",
                SellerName = p.User != null ? $"{p.User.FirstName} {p.User.LastName}".Trim() : "Bilinmeyen Satıcı",
                ImageUrl = string.IsNullOrEmpty(p.ImageUrl) ? "https://via.placeholder.com/100" : p.ImageUrl,
                StockQuantity = p.StockQuantity
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
                // Ürün görselini sunucudan sil.
                //TODO Yönetici paneli ve Satıcı ilan paneli aynı edit ve delete fonksiyonlarını kullanıyor. Kod tekrarını azaltmak adına ilerde göz atmalı.
                if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl.StartsWith("/images/products/"))
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _productRepository.Delete(product);
                await _productRepository.SaveChangesAsync();

                TempData["SuccessMessage"] = "Ürün sistemden kalıcı olarak silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}