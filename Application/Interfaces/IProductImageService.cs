using Application.DTOs.Requests;
using Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProductImageService
    {
        Task<bool> CreateAsync(CreateProductImageRequest request);
        Task<bool> DeleteAsync(string imageId);
        Task<IEnumerable<ProductImageResponse>> GetByProductIdAsync(string productId);
    }

}
