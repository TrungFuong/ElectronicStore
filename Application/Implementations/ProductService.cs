using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models.Responses;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        // 1. Tạo sản phẩm
        public async Task<bool> CreateProductAsync(CreateProductRequest request)
        {
            try
            {
                var product = new Product
                {
                    ProductId = Guid.NewGuid().ToString(),
                    ProductName = request.ProductName,
                    ProductDescription = request.ProductDescription,
                    ProductPrice = request.ProductPrice,
                    StockQuantity = request.StockQuantity,
                    CategoryId = request.CategoryId,
                    BrandId = request.BrandId,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    IsActive = true
                };

                await _productRepository.AddAsync(product);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 2. Lấy danh sách sản phẩm
        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var products = await _productRepository.GetAllWithIncludesAsync();
            return products.Select(MapToResponse);
        }

        // 3. Lấy chi tiết sản phẩm
        public async Task<ProductResponse?> GetByIdAsync(string productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return null;

            return MapToResponse(product);
        }

        // 4. Cập nhật sản phẩm
        public async Task<bool> UpdateProductAsync(UpdateProductRequest request)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(request.ProductId);
                if (product == null) return false;

                product.ProductName = request.ProductName;
                product.ProductDescription = request.ProductDescription;
                product.ProductPrice = request.ProductPrice;
                product.StockQuantity = request.StockQuantity;
                product.CategoryId = request.CategoryId;
                product.BrandId = request.BrandId;
                product.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

                _productRepository.Update(product);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // 5. Xoá mềm sản phẩm
        public async Task<bool> DeleteProductAsync(string productId)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null) return false;

                _productRepository.SoftDelete(product);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ===== PRIVATE MAP =====
        private static ProductResponse MapToResponse(Product product)
        {
            return new ProductResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                StockQuantity = product.StockQuantity,
                ProductPrice = product.ProductPrice,
                IsActive = product.IsActive,
                CategoryName = product.Category?.CategoryName,
                BrandName = product.Brand?.BrandName
            };
        }
    }
}
