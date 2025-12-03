using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRefreshTokenRepository : IGenericsRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetByAccountIdAsync(string accountId);
        //Task RevokeAsync(RefreshToken token);
        //Task RevokeAllForUserAsync(string accountId);

    }
}
