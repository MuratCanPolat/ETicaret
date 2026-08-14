using System.ComponentModel.DataAnnotations;

namespace ETicaretWeb.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Lütfen ürünün adını giriniz.")]
        [StringLength(100, ErrorMessage = "Ürün adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Ürün Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Fiyat alanı zorunludur.")]
        [Range(1, 1000000, ErrorMessage = "Lütfen geçerli bir fiyat giriniz (Min: 1 ₺).")]
        [Display(Name = "Fiyat (₺)")]
        public decimal Price { get; set; }

        //TODO İleride buraya Kategori seçimi (Dropdown) ve Görsel yükleme eklenecek.
    }
}