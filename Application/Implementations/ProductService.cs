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

        //PRIVATE: GENERATE PRODUCT ID
        private async Task<string> GenerateProductIdAsync()
        {
            var lastProduct = (await _unitOfWork.ProductRepository.GetAllAsync())
                .OrderByDescending(p => p.ProductId)
                .FirstOrDefault();

            if (lastProduct == null)
                return Prefixes.PRODUCT_ID_PREFIX + "0001";

            var numberPart = lastProduct.ProductId
                .Substring(Prefixes.PRODUCT_ID_PREFIX.Length);

            var nextNumber = int.Parse(numberPart) + 1;

            return Prefixes.PRODUCT_ID_PREFIX + nextNumber.ToString("D4");
        }

        //CREATE PRODUCT
        public async Task<bool> CreateProductAsync(CreateProductRequest request)
        {
            var brand = await _unitOfWork.BrandRepository
                .GetAsync(b => b.BrandId == request.BrandId);

            var category = await _unitOfWork.CategoryRepository
                .GetAsync(c => c.CategoryId == request.CategoryId);

            if (brand == null || category == null)
                throw new Exception("Brand hoặc Category không tồn tại");

            var product = new Product
            {
                ProductId = await GenerateProductIdAsync(),
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

        //GET ALL (CÓ ẢNH MAIN)
        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var products = await _unitOfWork.ProductRepository
                .GetAllAsync(p => p.IsActive, p => p.Category, p => p.Brand);

            var productIds = products.Select(p => p.ProductId).ToList();

            var mainImages = await _unitOfWork.ProductImageRepository
                .GetAllAsync(i => productIds.Contains(i.ProductId) && i.IsMain);

            return products.Select(p => new ProductResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice,
                StockQuantity = p.StockQuantity,
                ProductDescription = p.ProductDescription,
                CategoryName = p.Category?.CategoryName,
                BrandName = p.Brand?.BrandName,

                Images = mainImages
                    .Where(i => i.ProductId == p.ProductId)
                    .Select(i => new ProductImageResponse
                    {
                        ImageUrl = i.ImageUrl,
                        IsMain = true
                    })
                    .ToList(),

                Variations = null,
                Specifications = null
            });
        }

        //GET BY ID (FULL DETAIL)
        public async Task<ProductResponse?> GetByIdAsync(string productId)
        {
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == productId && p.IsActive,
                          p => p.Category,
                          p => p.Brand);

            if (product == null) return null;

            var variations = await _unitOfWork.ProductVariationRepository
                .GetAllAsync(v => v.ProductId == productId, v => v.Options);

            var attributes = await _unitOfWork.VariationAttributeRepository
                .GetAllAsync();

            var specifications = await _unitOfWork.ProductSpecificationRepository
                .GetAllAsync(s => s.ProductId == productId);

            var images = await _unitOfWork.ProductImageRepository
                .GetAllAsync(i => i.ProductId == productId);

            return new ProductResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                StockQuantity = product.StockQuantity,
                ProductDescription = product.ProductDescription,
                CategoryName = product.Category?.CategoryName,
                BrandName = product.Brand?.BrandName,

                Variations = variations.Select(v => new ProductVariationResponse
                {
                    VariationId = v.VariationId,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,
                    Options = v.Options.Select(o => new VariationOptionResponse
                    {
                        AttributeId = o.AttributeId,
                        AttributeName = attributes
                            .FirstOrDefault(a => a.AttributeId == o.AttributeId)?.Name ?? "",
                        Value = o.Value
                    }).ToList()
                }).ToList(),

                Specifications = specifications.Select(s => new ProductSpecificationResponse
                {
                    SpecificationId = s.SpecificationId,
                    SpecKey = s.SpecKey,
                    SpecValue = s.SpecValue
                }).ToList(),

                Images = images.Select(i => new ProductImageResponse
                {
                    ImageId = i.ImageId,
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain
                }).ToList()
            };
        }

        //UPDATE
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

        //DELETE (SOFT)
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

        //GET BY CATEGORY 
        public async Task<IEnumerable<ProductCardResponse>> GetByCategoryAsync(string categoryId)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                p => p.IsActive && p.CategoryId == categoryId,
                p => p.Category,
                p => p.Brand
            );

            return products.Select(p => new ProductCardResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice,
                StockQuantity = p.StockQuantity,
                ProductDescription = p.ProductDescription,
                
                BrandName = p.Brand?.BrandName,
                
                CategoryName = p.Category?.CategoryName,
                ImageUrl = p.Images?
                .FirstOrDefault(i => i.IsMain)?.ImageUrl
            });
        }
        public async Task<IEnumerable<ProductCardResponse>> GetByBrandAsync(string brandId)
        {
            var products = await _unitOfWork.ProductRepository
                .GetAllAsync(p =>
                    p.BrandId == brandId &&
                    p.IsActive);

            return products.Select(p => new ProductCardResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductPrice = p.ProductPrice,
                StockQuantity = p.StockQuantity,
                ProductDescription = p.ProductDescription,

                BrandName = p.Brand?.BrandName,

                CategoryName = p.Category?.CategoryName,
                ImageUrl = p.Images?
                .FirstOrDefault(i => i.IsMain)?.ImageUrl
            });
        }
    }
}
