using ECommerce513.Models;

namespace ECommerce513.Repository.IRepository
{
    public interface IOrderItemRepository : IRepository<OrderItems>
    {
        Task<bool> CreateRangeAsync(IEnumerable<OrderItems> entities);
    }
}
