using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductVariationRepository : GenericRepository<ProductVariation>, IProductVariationRepository
    {
        private readonly DBContext _db;

        public ProductVariationRepository(DBContext db) : base(db)
        {
            _db = db;
        }

        public async Task<ProductVariation?> GetByIdAsync(string variationId)
        {
            return await _db.ProductVariations
                .FirstOrDefaultAsync(v => v.VariationId == variationId);
        }
    }
}
