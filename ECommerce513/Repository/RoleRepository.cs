using ECommerce513.Data;
using ECommerce513.Models;
using ECommerce513.Repository.IRepository;
using Microsoft.AspNetCore.Identity;

namespace ECommerce513.Repository
{
    public class RoleRepository : Repository<IdentityRole>, IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
