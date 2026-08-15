using E_Ticaret.Models;
using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using ETicaretWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace E_Ticaret.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Product> _productRepository;

        public HomeController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();

            var viewModelList = products.Take(8).Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                FormattedPrice = p.Price.ToString("C"),
                ImageUrl = string.IsNullOrEmpty(p.ImageUrl) ? "https://via.placeholder.com/300x200?text=Gorsel+Yok" : p.ImageUrl,
                SellerName = p.User?.FirstName ?? "Satıcı"
            }).ToList();

            return View(viewModelList);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
