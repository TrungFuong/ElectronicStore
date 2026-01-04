using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DBContext _context;
        public OrderRepository(DBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order?> GetByIdWithDetailsAsync(string orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variation)
                        .ThenInclude(v => v.Product)
                            .ThenInclude(p => p.Images)

                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variation)
                        .ThenInclude(v => v.Options)

                .Include(o => o.DiscountUsages)
                .Include(o => o.Payments)

                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
        public async Task<IEnumerable<Order>> GetByAccountIdAsync(string accountId)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Variation)
                        .ThenInclude(v => v.Product)
                .Where(o => o.Customer.AccountId == accountId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
