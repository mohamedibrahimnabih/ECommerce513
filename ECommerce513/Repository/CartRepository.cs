using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ECommerce513.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> DeleteRangeAsync(IEnumerable<Cart> entities)
        {
            try
            {

                _context.Carts.RemoveRange(entities);
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
