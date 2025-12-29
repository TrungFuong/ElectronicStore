using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly DBContext _db;
        public CartItemRepository(DBContext db) => _db = db;

        public Task<CartItem?> GetByCartAndVariationAsync(string cartId, string variationId)
            => _db.CartItems.FirstOrDefaultAsync(x => x.CartId == cartId && x.VariationId == variationId);

        public async Task<List<CartItem>> GetByCartIdAsync(string cartId, bool includeVariation = false)
        {
            var q = _db.CartItems.Where(x => x.CartId == cartId);
            if (includeVariation) q = q.Include(x => x.Variation);
            return await q.ToListAsync();
        }

        public Task AddAsync(CartItem item) => _db.CartItems.AddAsync(item).AsTask();

        public void Update(CartItem item) => _db.CartItems.Update(item);

        public void Remove(CartItem item) => _db.CartItems.Remove(item);

        public void RemoveRange(IEnumerable<CartItem> items) => _db.CartItems.RemoveRange(items);
    }
}
