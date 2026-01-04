using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICustomerService
    {
        Task<string> CreateAsync(CreateCustomerRequest request, string accountId);
        Task<IEnumerable<CustomerResponse>> GetAllAsync();
        Task<CustomerResponse?> GetByIdAsync(string customerId);
        Task<bool> UpdateAsync(UpdateCustomerRequest request);
        Task<bool> DeleteAsync(string customerId);

        Task<CustomerResponse?> GetByAccountIdAsync(string accountId);
    }
}
