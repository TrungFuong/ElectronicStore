using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByAccountIdAsync(string accountId);
        Task<Cart?> GetByIdAsync(string cartId);
        Task AddAsync(Cart cart);
        void Update(Cart cart);
    }
}
