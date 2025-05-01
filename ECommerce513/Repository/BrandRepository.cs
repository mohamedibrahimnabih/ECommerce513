using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository.IRepository;

namespace ECommerce513.Repository
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        private readonly ApplicationDbContext _context;

        public BrandRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
