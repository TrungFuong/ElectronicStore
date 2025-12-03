using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAccountRepository : IGenericsRepository<Account>
    {
        // Dùng cho login
        Task<Account?> GetByPhoneAsync(string phone);

        // Dùng cho refresh token
        Task<Account?> GetWithRefreshTokensAsync(string accountId);
    }
}