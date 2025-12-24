using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductSpecificationService
    {
        Task<bool> CreateAsync(CreateProductSpecificationRequest request);
        Task<bool> UpdateAsync(UpdateProductSpecificationRequest request);
        Task<bool> DeleteAsync(string specificationId);

        Task<IEnumerable<ProductSpecificationResponse>>
        GetByProductIdAsync(string productId);
    }

}
