using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDiscountRepository : IGenericsRepository<Discount>
    {
        Task<Discount?> GetByCodeAsync(string code);
    }
}
