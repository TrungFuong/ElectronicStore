using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly DBContext _context;
        public AccountRepository(DBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(string accountId)
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        public async Task<Account?> GetByPhoneAsync(string phone)
        {
            var all = await _context.Accounts.ToListAsync();
            foreach (var acc in all)
            {
                Console.WriteLine($"DB PHONE: '{acc.Phone}'");
            }

            return await _context.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.Phone == phone);
        }

        public async Task<Account?> GetWithRefreshTokensAsync(string accountId)
        {
            return await _context.Accounts
                .Include(a => a.RefreshTokens)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);
        }
    }
}