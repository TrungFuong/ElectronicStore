using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDiscountService
    {
        Task<bool> CreateDiscountAsync(CreateDiscountRequest request);
        Task<IEnumerable<DiscountResponse>> GetAllAsync();
        Task<DiscountResponse?> GetByIdAsync(string discountId);
        Task<bool> UpdateDiscountAsync(UpdateDiscountRequest request);
        Task<bool> DeleteDiscountAsync(string discountId);
    }
}
