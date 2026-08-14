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

            //TODO İleride buraya ".OrderByDescending(x => x.Id).Take(8)" ekleyerek sadece en yeni 8 ilanı getirilmesi sağlanabilir.

            var viewModelList = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                FormattedPrice = p.Price.ToString("C"),
                SellerName = "Sistem Yöneticisi"
            }).ToList();

            return View(viewModelList);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
