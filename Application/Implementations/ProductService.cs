using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Models.Responses;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // CREATE
        public async Task<bool> CreateProductAsync(CreateProductRequest request)
        {
            var count = await _unitOfWork.ProductRepository.CountAsync();
            var product = new Product
            {
                ProductId = Prefixes.PRODUCT_ID_PREFIX + string.Format(Prefixes.ID_FORMAT, count + 1),
                ProductName = request.ProductName,
                ProductPrice = request.ProductPrice,
                StockQuantity = request.StockQuantity,
                ProductDescription = request.ProductDescription,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                IsActive = true
            };

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // GET ALL
        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var products = await _unitOfWork.ProductRepository
                .GetAllAsync(p => p.IsActive, p => p.Category, p => p.Brand);

            return products.Select(p => new ProductResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice,
                StockQuantity = p.StockQuantity,
                ProductDescription = p.ProductDescription,
                CategoryName = p.Category?.CategoryName,
                BrandName = p.Brand?.BrandName
            });
        }

        // GET BY ID
        public async Task<ProductResponse?> GetByIdAsync(string productId)
        {
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == productId && p.IsActive,
                          p => p.Category, p => p.Brand);

            if (product == null) return null;

            return new ProductResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                StockQuantity = product.StockQuantity,
                ProductDescription = product.ProductDescription,
                CategoryName = product.Category?.CategoryName,
                BrandName = product.Brand?.BrandName
            };
        }

        // UPDATE
        public async Task<bool> UpdateProductAsync(UpdateProductRequest request)
        {
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == request.ProductId && p.IsActive);

            if (product == null) return false;

            product.ProductName = request.ProductName;
            product.ProductPrice = request.ProductPrice;
            product.StockQuantity = request.StockQuantity;
            product.ProductDescription = request.ProductDescription;
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;
            product.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.CommitAsync();

            return true;
        }

        // SOFT DELETE
        public async Task<bool> DeleteProductAsync(string productId)
        {
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == productId && p.IsActive);

            if (product == null) return false;

            product.IsActive = false;
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.CommitAsync();

            return true;
        }
        // GET CATEGORY
        public async Task<IEnumerable<CategoryProductResponse>> GetByCategoryAsync(string categoryId)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                p => p.CategoryId == categoryId && p.IsActive,
                p => p.Brand,
                p => p.Category
            );

            return products.Select(p => new CategoryProductResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice,
                StockQuantity = p.StockQuantity,
                ProductDescription = p.ProductDescription,

                BrandId = p.BrandId,
                BrandName = p.Brand?.BrandName,

                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName
            });
        }

    }
}
