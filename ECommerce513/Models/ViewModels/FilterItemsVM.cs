﻿namespace ECommerce513.Models.ViewModels
{
    public class FilterItemsVM
    {
        public string? ProductName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int CategoryId { get; set; }
    }
}
