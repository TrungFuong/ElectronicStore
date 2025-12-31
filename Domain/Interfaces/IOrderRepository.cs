using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IOrderRepository : IGenericsRepository<Order>
    {
        Task<Order?> GetByIdWithDetailsAsync(string orderId);
    }
}
