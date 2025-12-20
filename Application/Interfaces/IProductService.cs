using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Interfaces
{
    public interface IProductService
    {
        // Tạo sản phẩm (Admin, Staff)
        Task<bool> CreateProductAsync(CreateProductRequest request);

        // Lấy danh sách sản phẩm
        Task<IEnumerable<ProductResponse>> GetAllAsync();

        // Lấy chi tiết sản phẩm
        Task<ProductResponse?> GetByIdAsync(string productId);

        // Cập nhật sản phẩm
        Task<bool> UpdateProductAsync(UpdateProductRequest request);

        // Xoá mềm sản phẩm
        Task<bool> DeleteProductAsync(string productId);

        // lay danh muc
        Task<IEnumerable<CategoryProductResponse>> GetByCategoryAsync(string categoryId);

    }
}
