using E_Ticaret.Extensions;
using E_Ticaret.ViewModels;
using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Ticaret.Controllers
{
    public class CartController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private const string CartSessionKey = "MyCart";

        public CartController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        // Sepet Görüntüleme.
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetJson<List<CartItemViewModel>>(CartSessionKey) ?? new List<CartItemViewModel>();

            return View(cart);
        }

        // Sepete Ekleme.
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return NotFound();

            var cart = HttpContext.Session.GetJson<List<CartItemViewModel>>(CartSessionKey) ?? new List<CartItemViewModel>();

            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    ImageUrl = string.IsNullOrEmpty(product.ImageUrl) ? "https://via.placeholder.com/100" : product.ImageUrl, //TODO Projenin ilk aşamasında ürün resimlerini internetten bulduğum resimlerle doldurmaya çalıştığımdan property isimleri de imageurl olarak kaldı ilerleyen dönemde bunu düzeltmeliyim.
                    Quantity = quantity
                });
            }

            HttpContext.Session.SetJson(CartSessionKey, cart);

            TempData["SuccessMessage"] = $"{product.Name} sepete eklendi!";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        // Sepetten Ürün Çıkarma.
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemViewModel>>(CartSessionKey);

            if (cart != null)
            {
                cart.RemoveAll(c => c.ProductId == productId);
                HttpContext.Session.SetJson(CartSessionKey, cart);
            }

            return RedirectToAction(nameof(Index));
        }

        // Sepeti Tamamen Temizleme.
        [HttpPost]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction(nameof(Index));
        }
    }
}
