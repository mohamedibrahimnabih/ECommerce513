using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository.IRepository;

namespace ECommerce513.Repository
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
