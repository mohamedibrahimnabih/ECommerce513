using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository.IRepository;

namespace ECommerce513.Repository
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
