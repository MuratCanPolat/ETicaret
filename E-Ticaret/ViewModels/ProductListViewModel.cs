namespace ETicaretWeb.ViewModels
{
    public class ProductListViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string FormattedPrice { get; set; } = string.Empty;

        public string SellerName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = "https://via.placeholder.com/300x200?text=Gorsel+Yok";
    }
}