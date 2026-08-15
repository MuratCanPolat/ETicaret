using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ETicaretWeb.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Lütfen ürünün adını giriniz.")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ürün açıklaması zorunludur.")]
        [StringLength(300, ErrorMessage = "Açıklama en fazla 300 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat alanı zorunludur.")]
        [Range(1, 1000000, ErrorMessage = "Lütfen geçerli bir fiyat giriniz (Min: 1 ₺).")]
        [Display(Name = "Fiyat (₺)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok miktarı zorunludur.")]
        [Range(0, int.MaxValue, ErrorMessage = "Geçerli bir stok miktarı giriniz.")]
        [Display(Name = "Stok Adedi")]
        public int StockQuantity { get; set; }

        [Display(Name = "Ürün Görseli")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        public string? ExistingImageUrl { get; set; }

        //TODO İleride buraya Kategori seçimi (Dropdown) ve Görsel yükleme eklenecek.(Tamamlandı)
    }
}