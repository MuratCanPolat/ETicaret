using ETicaret.Core.Entities;
using ETicaret.Core.Interfaces;
using ETicaretWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ETicaretWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IRepository<Product> productRepository, IRepository<Category> categoryRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment;
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
                Description = p.Description,
                FormattedPrice = p.Price.ToString("C"),
                ImageUrl = string.IsNullOrEmpty(p.ImageUrl) ? "https://via.placeholder.com/300x200?text=Gorsel+Yok" : p.ImageUrl,
                CategoryName = p.Category?.Name ?? "Kategori Yok",
                SellerName = p.User?.FirstName ?? "Satıcı"
            }).ToList();

            // View'a ViewModel gönderme.
            return View(viewModelList);
        }

        [Authorize(Roles = "Admin,Satıcı")]
        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");

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
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                string uniqueFileName = "";

                if (model.ImageFile != null)
                {
                    // Kaydedeceğimiz klasör yolu: wwwroot/images/products
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                    // Eğer klasör yoksa oluştur
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Aynı isimde iki dosya yüklenirse çakışmasın diye ismin başına ekliyoruz
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;

                    // Dosyanın tam kaydedileceği yol
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Dosyayı "sunucu"ya fiziksel olarak kopyalıyoruz
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    StockQuantity = model.StockQuantity,
                    CategoryId = model.CategoryId,
                    ImageUrl = "/images/products/" + uniqueFileName,
                    UserId = currentUserId!
                    //TODO Şimdilik UserId eklenmedi, Identity tam bağlandığında burayı güncellemeli. (Tamamlandı)
                };

                // Repository üzerinden veritabanına kaydetme.
                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();

                // Başarılı olursa listeleme (Index) sayfasına geri gönder.
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            // Eğer kurallara uyulmadıysa, formu hatalarla birlikte kullanıcıya geri göster.
            return View(model);
        }

        [Authorize(Roles = "Admin,Satıcı")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // Yetki kontrolü: Admin değilse ve ürünün sahibi giriş yapan kullanıcı değilse engelle.
            if (!isAdmin && product.UserId != currentUserId)
            {
                return Forbid();
            }

            var model = new ProductCreateViewModel
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId,
            };

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);

            ViewBag.ProductId = product.Id;
            ViewBag.ExistingImageUrl = product.ImageUrl;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Satıcı")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCreateViewModel model, string? existingImageUrl)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && product.UserId != currentUserId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                string imageUrl = existingImageUrl ?? "";

                // Eğer kullanıcı yeni bir görsel yüklediyse eskisiyle değiştir.
                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }
                    imageUrl = "/images/products/" + uniqueFileName;
                }

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.StockQuantity = model.StockQuantity;
                product.CategoryId = model.CategoryId;
                product.ImageUrl = imageUrl;

                 _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
            ViewBag.ProductId = id;
            ViewBag.ExistingImageUrl = existingImageUrl;

            return View(model);
        }
    }
}