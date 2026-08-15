using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();
            var totalUsers = _userManager.Users.Count();

            ViewBag.TotalProducts = products.Count();
            ViewBag.TotalCategories = categories.Count();
            ViewBag.TotalUsers = totalUsers;

            return View();
        }
    }
}