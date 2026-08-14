using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using ETicaretWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;

        public ProductsController(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index()
        {
            // Ürünleri çekme.
            var products = await _productRepository.GetAllAsync();

            // Verileri VieWModel'e dönüştürme.
            var viewModelList = products.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                // Fiyatı string formatına çevirme.
                FormattedPrice = p.Price.ToString("C"),
                // Şimdilik sabit bir satıcı ismi.
                SellerName = "Sistem Yöneticisi"
            }).ToList();

            // View'a ViewModel gönderme.
            return View(viewModelList);
        }
        [Authorize(Roles = "Admin,Satıcı")]
        public IActionResult Create()
        {
            // Ekrana boş form gönderme.
            return View(new ProductCreateViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Satıcı")]
        [ValidateAntiForgeryToken] // CSRF saldırılarına karşı form güvenliği sağlar. Siteler Arası İstek Sahteciliği bir kullanıcının web sitesinde açık olan
                                   // oturumunu kötüye kullanarak, onun haberi ve onayı olmadan o site üzerinde yetkisiz işlemler (para transferi, şifre değişimi vb.) yapılmasını sağlayan bir siber saldırı türüdür.
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            // Model kuralları sağlanıyor mu?
            if (ModelState.IsValid)
            {
                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price
                    //TODO Şimdilik UserId eklenmedi, Identity tam bağlandığında burayı güncellemeli.
                };

                // Repository üzerinden veritabanına kaydetme.
                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                // Başarılı olursa listeleme (Index) sayfasına geri gönder.
                return RedirectToAction(nameof(Index));
            }

            // Eğer kurallara uyulmadıysa, formu hatalarla birlikte kullanıcıya geri göster.
            return View(model);
        }
    }
}