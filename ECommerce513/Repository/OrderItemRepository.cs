using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce513.Repository
{
    public class OrderItemRepository : Repository<OrderItems>, IOrderItemRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderItemRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CreateRangeAsync(IEnumerable<OrderItems> entities)
        {
            try
            {

                await _context.OrderItems.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {

                Console.WriteLine($"{ex.Message}");
                return false;

            }
        }
    }
}
