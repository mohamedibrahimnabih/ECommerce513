using ECommerce513.Models;

namespace ECommerce513.Repository.IRepository
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<bool> DeleteRangeAsync(IEnumerable<Cart> entities);
    }
}
