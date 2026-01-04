using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IStaffRepository : IGenericsRepository<Staff>
    {
        // Get staff including Account navigation
        Task<Staff?> GetByIdWithAccountAsync(string staffId);

        // Find staff by associated AccountId
        Task<Staff?> GetByAccountIdAsync(string accountId);

        // Find staff by phone number
        Task<Staff?> GetByPhoneAsync(string phone);
    }
}
