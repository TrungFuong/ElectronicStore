using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class DiscountRepository : GenericRepository<Discount>, IDiscountRepository
    {
        private readonly DBContext _context;

        public DiscountRepository(DBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Discount?> GetByCodeAsync(string code)
        {
            return await _context.Discounts.FirstOrDefaultAsync(d => d.DiscountCode == code);
        }
    }
}
