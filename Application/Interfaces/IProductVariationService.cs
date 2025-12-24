using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductVariationService
    {
        Task<bool> CreateAsync(CreateProductVariationRequest request);
        Task<bool> UpdateAsync(UpdateProductVariationRequest request);
        Task<bool> DeleteAsync(string variationId);

        Task<IEnumerable<ProductVariationResponse>> GetByProductIdAsync(string productId);
        Task<ProductVariationResponse?> GetByIdAsync(string variationId);
    }

}
