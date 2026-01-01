using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetByCartAndVariationAsync(string cartId, string variationId);
        Task<List<CartItem>> GetByCartIdAsync(string cartId, bool includeVariation = false);
        Task AddAsync(CartItem item);
        void Update(CartItem item);
        void Remove(CartItem item);
        void RemoveRange(IEnumerable<CartItem> items);
    }
}
