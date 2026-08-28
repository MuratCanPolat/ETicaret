namespace E_Ticaret.ViewModels
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Sepetteki ürünlerin toplam fiyatını hesaplama.
        public decimal TotalPrice => Price * Quantity;
    }
}
