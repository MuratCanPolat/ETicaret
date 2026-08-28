using System.ComponentModel.DataAnnotations;

namespace E_Ticaret.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Lütfen teslimat adresi giriniz.")]
        [Display(Name = "Teslimat Adresi")]
        public string ShippingAddress { get; set; } = string.Empty;

        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal GrandTotal { get; set; }
    }
}
