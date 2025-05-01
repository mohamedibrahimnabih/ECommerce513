using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository.IRepository;

namespace ECommerce513.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
