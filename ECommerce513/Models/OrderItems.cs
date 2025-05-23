﻿using Microsoft.EntityFrameworkCore;

namespace ECommerce513.Models
{
    [PrimaryKey(nameof(OrderId), nameof(ProductId))]
    public class OrderItems
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public decimal Price { get; set; }
        public int Count { get; set; }
    }
}
