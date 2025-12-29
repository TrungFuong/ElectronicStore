using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly DBContext _db;
        public CartRepository(DBContext db) => _db = db;

        public Task<Cart?> GetByAccountIdAsync(string accountId)
            => _db.Carts.FirstOrDefaultAsync(c => c.AccountId == accountId);

        public Task<Cart?> GetByIdAsync(string cartId)
            => _db.Carts.FirstOrDefaultAsync(c => c.CartId == cartId);

        public Task AddAsync(Cart cart) => _db.Carts.AddAsync(cart).AsTask();

        public void Update(Cart cart) => _db.Carts.Update(cart);
    }
}
