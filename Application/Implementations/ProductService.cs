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

        // Generate id by prefix + running number (root only)
        private async Task<string> GenerateIdAsync<TEntity>(
           Func<IUnitOfWork, IGenericsRepository<TEntity>> repoSelector,
           Func<TEntity, string> idSelector,
           string prefix
       ) where TEntity : class
        {
            var repo = repoSelector(_unitOfWork);
            var last = (await repo.GetAllAsync())
                .OrderByDescending(e => idSelector(e))
                .FirstOrDefault();

            if (last == null)
                return prefix + string.Format(Prefixes.ID_FORMAT, 1);

            var num = int.Parse(idSelector(last).Substring(prefix.Length)) + 1;
            return prefix + string.Format(Prefixes.ID_FORMAT, num);
        }

        // ================= CREATE =================
        public async Task<bool> CreateProductAsync(CreateProductRequest request)
        {
            if (!request.Variations.Any())
                throw new Exception("Product phải có ít nhất 1 variation");

            if (request.Images.Count(i => i.IsMain) > 1)
                throw new Exception("Chỉ được phép 1 ảnh main");

            if (await _unitOfWork.BrandRepository.GetAsync(b => b.BrandId == request.BrandId) == null ||
                await _unitOfWork.CategoryRepository.GetAsync(c => c.CategoryId == request.CategoryId) == null)
                throw new Exception("Brand hoặc Category không tồn tại");

            var productId = await GenerateIdAsync(
                u => u.ProductRepository,
                p => p.ProductId,
                Prefixes.PRODUCT_ID_PREFIX);

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _unitOfWork.ProductRepository.AddAsync(new Product
                {
                    ProductId = productId,
                    ProductName = request.ProductName,
                    ProductDescription = request.ProductDescription,
                    CategoryId = request.CategoryId,
                    BrandId = request.BrandId,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now),
                    IsActive = true
                });

                // -------- VARIATION + OPTION --------

                // 🔹 LẤY VARIATION ID 1 LẦN
                var lastVariationId = await GenerateIdAsync(
                    u => u.ProductVariationRepository,
                    x => x.VariationId,
                    Prefixes.VARIATION_ID_PREFIX);

                var variationIndex =
                    int.Parse(lastVariationId.Substring(Prefixes.VARIATION_ID_PREFIX.Length));

                // 🔹 LẤY OPTION ID 1 LẦN
                var lastOptionId = await GenerateIdAsync(
                    u => u.VariationOptionRepository,
                    o => o.OptionId,
                    Prefixes.OPTION_ID_PREFIX);

                var optionIndex =
                    int.Parse(lastOptionId.Substring(Prefixes.OPTION_ID_PREFIX.Length));

                foreach (var v in request.Variations)
                {
                    // ===== CREATE VARIATION =====
                    variationIndex++;

                    var variationId = Prefixes.VARIATION_ID_PREFIX
                                      + string.Format(Prefixes.ID_FORMAT, variationIndex);

                    await _unitOfWork.ProductVariationRepository.AddAsync(
                        new ProductVariation
                        {
                            VariationId = variationId,
                            ProductId = productId,
                            Price = v.Price,
                            StockQuantity = v.StockQuantity
                        });

                    // ===== CREATE OPTIONS =====
                    foreach (var opt in v.Options)
                    {
                        optionIndex++;

                        await _unitOfWork.VariationOptionRepository.AddAsync(
                            new VariationOption
                            {
                                OptionId = Prefixes.OPTION_ID_PREFIX
                                           + string.Format(Prefixes.ID_FORMAT, optionIndex),
                                VariationId = variationId,
                                AttributeId = opt.AttributeId,
                                Value = opt.OptionValue
                            });
                    }
                }


                // -------- SPECIFICATION --------
                var lastSpecId = await GenerateIdAsync(
                    u => u.ProductSpecificationRepository,
                    s => s.SpecificationId,
                    Prefixes.SPECIFICATION_ID_PREFIX);
                var specIndex = int.Parse(lastSpecId.Substring(Prefixes.SPECIFICATION_ID_PREFIX.Length));

                foreach (var s in request.Specifications)
                {
                    specIndex++;
                    await _unitOfWork.ProductSpecificationRepository.AddAsync(
                        new ProductSpecification
                        {
                            SpecificationId = Prefixes.SPECIFICATION_ID_PREFIX
                                              + string.Format(Prefixes.ID_FORMAT, specIndex),
                            ProductId = productId,
                            SpecKey = s.SpecKey,
                            SpecValue = s.SpecValue
                        });
                }

                // -------- IMAGE --------
                var lastImgId = await GenerateIdAsync(
                    u => u.ProductImageRepository,
                    i => i.ImageId,
                    Prefixes.IMAGE_ID_PREFIX);
                var imgIndex = int.Parse(lastImgId.Substring(Prefixes.IMAGE_ID_PREFIX.Length));

                foreach (var img in request.Images)
                {
                    imgIndex++;
                    await _unitOfWork.ProductImageRepository.AddAsync(
                        new ProductImage
                        {
                            ImageId = Prefixes.IMAGE_ID_PREFIX
                                      + string.Format(Prefixes.ID_FORMAT, imgIndex),
                            ProductId = productId,
                            ImageUrl = img.ImageUrl,
                            IsMain = img.IsMain,
                            IsActive = true
                        });
                }

            });
            return true;



        }

        // ========================= READ =========================
        public async Task<IEnumerable<ProductResponse>> GetAllAsync()
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                p => p.IsActive,
                p => p.Category,
                p => p.Brand,
                p => p.Images,
                p => p.Variations
            );

            var variationIds = products
                .SelectMany(p => p.Variations)
                .Select(v => v.VariationId)
                .ToList();

            var options = await _unitOfWork.VariationOptionRepository
                .GetAllAsync(o => variationIds.Contains(o.VariationId));

            var attributes = await _unitOfWork.VariationAttributeRepository.GetAllAsync();

            var productIds = products.Select(p => p.ProductId).ToList();

            var specs = await _unitOfWork.ProductSpecificationRepository
                .GetAllAsync(s => productIds.Contains(s.ProductId));

            return products.Select(p => new ProductResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                CategoryName = p.Category?.CategoryName,
                BrandName = p.Brand?.BrandName,
                ProductPrice = p.Variations.Any() ? p.Variations.Min(v => v.Price) : 0,
                StockQuantity = p.TotalStock,

                Images = p.Images.Select(i => new ProductImageResponse
                {
                    ImageId = i.ImageId,
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain
                }).ToList(),

                Variations = p.Variations.Select(v => new ProductVariationResponse
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

                Specifications = specs
                    .Where(s => s.ProductId == p.ProductId)
                    .Select(s => new ProductSpecificationResponse
                    {
                        SpecificationId = s.SpecificationId,
                        SpecKey = s.SpecKey,
                        SpecValue = s.SpecValue
                    }).ToList()
            });
        }

        public async Task<ProductResponse?> GetByIdAsync(string productId)
        {
            var product = await _unitOfWork.ProductRepository.GetAsync(
                p => p.ProductId == productId && p.IsActive,
                p => p.Category, p => p.Brand, p => p.Images, p => p.Variations);

            if (product == null) return null;

            return new ProductResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                CategoryName = product.Category?.CategoryName,
                BrandName = product.Brand?.BrandName,
                ProductPrice = product.Variations.Any() ? product.Variations.Min(v => v.Price) : 0,
                StockQuantity = product.TotalStock,
                Images = product.Images.Select(i =>
                    new ProductImageResponse { ImageId = i.ImageId, ImageUrl = i.ImageUrl, IsMain = i.IsMain }
                ).ToList()
            };
        }

        // ========================= UPDATE =========================
        public async Task<bool> UpdateProductAsync(UpdateProductRequest request)
        {
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == request.ProductId && p.IsActive);
            if (product == null) return false;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (!string.IsNullOrWhiteSpace(request.ProductName))
                    product.ProductName = request.ProductName;
                if (!string.IsNullOrWhiteSpace(request.ProductDescription))
                    product.ProductDescription = request.ProductDescription;
                if (!string.IsNullOrWhiteSpace(request.CategoryId))
                    product.CategoryId = request.CategoryId;
                if (!string.IsNullOrWhiteSpace(request.BrandId))
                    product.BrandId = request.BrandId;

                product.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                _unitOfWork.ProductRepository.Update(product);

                // IMAGE UPDATE FIX
                var lastImgId = await GenerateIdAsync(
                    u => u.ProductImageRepository,
                    i => i.ImageId,
                    Prefixes.IMAGE_ID_PREFIX);
                var imgIndex = int.Parse(lastImgId.Substring(Prefixes.IMAGE_ID_PREFIX.Length));

                foreach (var img in request.Images)
                {
                    if (img.IsDeleted && img.ImageId != null)
                    {
                        var di = await _unitOfWork.ProductImageRepository
                            .GetAsync(i => i.ImageId == img.ImageId);
                        if (di != null) _unitOfWork.ProductImageRepository.Delete(di);
                        continue;
                    }

                    if (img.ImageId == null)
                    {
                        imgIndex++;
                        await _unitOfWork.ProductImageRepository.AddAsync(
                            new ProductImage
                            {
                                ImageId = Prefixes.IMAGE_ID_PREFIX
                                          + string.Format(Prefixes.ID_FORMAT, imgIndex),
                                ProductId = product.ProductId,
                                ImageUrl = img.ImageUrl,
                                IsMain = img.IsMain
                            });
                        continue;
                    }

                    var ei = await _unitOfWork.ProductImageRepository
                        .GetAsync(i => i.ImageId == img.ImageId);
                    if (ei == null) continue;

                    ei.ImageUrl = img.ImageUrl;
                    ei.IsMain = img.IsMain;
                    _unitOfWork.ProductImageRepository.Update(ei);
                }
            });

            return true;
        }

        // ========================= DELETE =========================
        public async Task<bool> DeleteProductAsync(string productId)
        {
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == productId && p.IsActive);
            if (product == null) return false;

            
            _unitOfWork.ProductRepository.SoftDelete(product);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // ========================= FILTER =========================
        public async Task<IEnumerable<ProductCardResponse>> GetByCategoryAsync(string categoryId)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                p => p.IsActive && p.CategoryId == categoryId,
                p => p.Category, p => p.Brand, p => p.Images, p => p.Variations);

            return products.Select(p => new ProductCardResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                CategoryName = p.Category?.CategoryName,
                BrandName = p.Brand?.BrandName,
                ProductPrice = p.Variations.Any() ? p.Variations.Min(v => v.Price) : 0,
                StockQuantity = p.TotalStock,
                ImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
            });
        }

        public async Task<IEnumerable<ProductCardResponse>> GetByBrandAsync(string brandId)
        {
            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                p => p.IsActive && p.BrandId == brandId,
                p => p.Category, p => p.Brand, p => p.Images, p => p.Variations);

            return products.Select(p => new ProductCardResponse
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                ProductDescription = p.ProductDescription,
                CategoryName = p.Category?.CategoryName,
                BrandName = p.Brand?.BrandName,
                ProductPrice = p.Variations.Any() ? p.Variations.Min(v => v.Price) : 0,
                StockQuantity = p.TotalStock,
                ImageUrl = p.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl
            });
        }
    }
}
