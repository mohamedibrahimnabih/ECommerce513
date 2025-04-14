namespace ECommerce513.Models.ViewModels
{
    public class ProductWithFilterVM
    {
        public FilterItemsVM FilterItemsVM { get; set; } = null!;
        public List<Product> Products { get; set; } = null!;
        public List<Category> Categories { get; set; } = null!;
    }
}
