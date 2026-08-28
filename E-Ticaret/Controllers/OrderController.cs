using E_Ticaret.Extensions;
using E_Ticaret.ViewModels;
using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Ticaret.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Product> _productRepository;
        private const string CartSessionKey = "MyCart";

        public OrderController(IRepository<Order> orderRepository, IRepository<Product> productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetJson<List<CartItemViewModel>>(CartSessionKey);

            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Sepetiniz boş. Lütfen önce ürün ekleyin.";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.TotalPrice)
            };

            return View(model);
        }

        // Siparişi veritanına kaydetme.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = HttpContext.Session.GetJson<List<CartItemViewModel>>(CartSessionKey);

            if (cart == null || !cart.Any()) return RedirectToAction("Index", "Cart");

            if (!ModelState.IsValid)
            {
                model.CartItems = cart;
                model.GrandTotal = cart.Sum(x => x.TotalPrice);
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = new Order
            {
                UserId = userId!,
                OrderDate = DateTime.Now,
                ShippingAddress = model.ShippingAddress,
                TotalAmount = cart.Sum(x => x.TotalPrice),
                OrderStatus = "Onay Bekliyor",
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cart)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);

                if (product != null)
                {
                    if (product.StockQuantity < item.Quantity)
                    {
                        ModelState.AddModelError("", $"{product.Name} için yeterli stok yok. Kalan: {product.StockQuantity}");
                        model.CartItems = cart;
                        model.GrandTotal = cart.Sum(x => x.TotalPrice);
                        return View(model);
                    }

                    product.StockQuantity -= item.Quantity;
                    _productRepository.Update(product);

                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Price,
                        ItemStatus = "Hazırlanıyor" 
                    });
                }
            }

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            HttpContext.Session.Remove(CartSessionKey);

            return RedirectToAction(nameof(Success), new { orderId = order.Id });
        }

        // Sipariş başarılı sayfası. Buraya makbuz vesayre konabilir.
        public IActionResult Success(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }
    }
}
