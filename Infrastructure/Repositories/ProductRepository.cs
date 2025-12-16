using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly DBContext _context;

        public ProductRepository(DBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(string productId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<IEnumerable<Product>> GetAllWithIncludesAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.IsActive)
                .ToListAsync();
        }
    }
}
