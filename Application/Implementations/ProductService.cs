using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // =========================
        // GENERATE PRODUCT ID
        // =========================
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

        // =========================
        // CREATE PRODUCT (STEP 1)
        // =========================
        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            var brand = await _unitOfWork.BrandRepository
                .GetAsync(b => b.BrandId == request.BrandId);

            var category = await _unitOfWork.CategoryRepository
                .GetAsync(c => c.CategoryId == request.CategoryId);

            if (brand == null || category == null)
                throw new Exception("Brand hoặc Category không tồn tại");

            var productId = await GenerateProductIdAsync();

            var product = new Product
            {
                ProductId = productId,
                ProductName = request.ProductName,
                ProductDescription = request.ProductDescription,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                IsActive = true
            };

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.CommitAsync();

            return new ProductResponse
            {
                ProductId = productId
            };
        }

        // =========================
        // GET ALL PRODUCTS (CARD)
        // =========================
        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                p => p.IsActive,
                p => p.Category,
                p => p.Brand,
                p => p.Images,
                p => p.Variations
            );

            return products.Select(p => new ProductResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                CategoryName = p.Category?.CategoryName,
                BrandName = p.Brand?.BrandName,

                ProductPrice = (p.Variations ?? new List<ProductVariation>())
                    .Any() ? p.Variations.Min(v => v.Price) : 0,

                StockQuantity = p.TotalStock,

                Images = (p.Images ?? new List<ProductImage>())
                    .Where(i => i.IsMain)
                    .Select(i => new ProductImageResponse
                    {
                        ImageUrl = i.ImageUrl,
                        IsMain = true
                    }).ToList()
            });
        }

        // =========================
        // GET PRODUCT BY ID (FULL)
        // =========================
        public async Task<ProductResponse?> GetByIdAsync(string productId)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(
                p => p.ProductId == productId && p.IsActive,
                p => p.Category,
                p => p.Brand,
                p => p.Images,
                p => p.Variations
            );

            if (product == null) return null;

            // 🔥 LOAD OPTIONS RIÊNG
            var variationIds = product.Variations.Select(v => v.VariationId).ToList();

            var options = await _unitOfWork.VariationOptionRepository
                .GetAllAsync(o => variationIds.Contains(o.VariationId));

            var attributes = await _unitOfWork.VariationAttributeRepository.GetAllAsync();

            var specifications = await _unitOfWork.ProductSpecificationRepository
                .GetAllAsync(s => s.ProductId == productId);

            return new ProductResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                CategoryName = product.Category?.CategoryName,
                BrandName = product.Brand?.BrandName,
                    
                ProductPrice = product.Variations.Any()
                    ? product.Variations.Min(v => v.Price)
                    : 0,

                StockQuantity = product.TotalStock,

                Variations = product.Variations.Select(v => new ProductVariationResponse
                {
                    VariationId = v.VariationId,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,

                    Options = options
                        .Where(o => o.VariationId == v.VariationId)
                        .Select(o => new VariationOptionResponse
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

                Images = product.Images.Select(i => new ProductImageResponse
                {
                    ImageId = i.ImageId,
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain
                }).ToList()
            };
        }


        // =========================
        // UPDATE PRODUCT
        // =========================
        public async Task<bool> UpdateProductAsync(UpdateProductRequest request)
        {
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == request.ProductId && p.IsActive);

            if (product == null) return false;

            product.ProductName = request.ProductName;
            product.ProductDescription = request.ProductDescription;
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;
            product.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // =========================
        // DELETE PRODUCT (SOFT)
        // =========================
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

        // =========================
        // GET BY CATEGORY
        // =========================
        public async Task<IEnumerable<ProductCardResponse>> GetByCategoryAsync(string categoryId)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                p => p.IsActive && p.CategoryId == categoryId,
                p => p.Category,
                p => p.Brand,
                p => p.Images,
                p => p.Variations
            );

            return products.Select(p => new ProductCardResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                CategoryName = p.Category?.CategoryName,
                BrandName = p.Brand?.BrandName,
                ProductPrice = (p.Variations ?? new List<ProductVariation>())
                    .Any() ? p.Variations.Min(v => v.Price) : 0,
                StockQuantity = p.TotalStock,
                ImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
            });
        }

        // =========================
        // GET BY BRAND
        // =========================
        public async Task<IEnumerable<ProductCardResponse>> GetByBrandAsync(string brandId)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                p => p.IsActive && p.BrandId == brandId,
                p => p.Category,
                p => p.Brand,
                p => p.Images,
                p => p.Variations
            );

            return products.Select(p => new ProductCardResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                CategoryName = p.Category?.CategoryName,
                BrandName = p.Brand?.BrandName,
                ProductPrice = (p.Variations ?? new List<ProductVariation>())
                    .Any() ? p.Variations.Min(v => v.Price) : 0,
                StockQuantity = p.TotalStock,
                ImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
            });
        }
    }
}
