namespace ECommerce513.Models.ViewModels
{
    public class FilterItemsVM
    {
        public string? ProductName { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public int CategoryId { get; set; }
    }
}
