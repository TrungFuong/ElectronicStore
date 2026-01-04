using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class StaffRepository : GenericRepository<Staff>, IStaffRepository
    {
        private readonly DBContext _context;

        public StaffRepository(DBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Staff?> GetByIdWithAccountAsync(string staffId)
        {
            return await _context.Staffs
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.StaffId == staffId);
        }

        public async Task<Staff?> GetByAccountIdAsync(string accountId)
        {
            return await _context.Staffs
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.AccountId == accountId);
        }

        public async Task<Staff?> GetByPhoneAsync(string phone)
        {
            return await _context.Staffs
                .Include(s => s.Account)
                .FirstOrDefaultAsync(s => s.Phone == phone);
        }
    }
}
