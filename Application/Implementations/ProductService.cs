using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
                    IsActive = request.IsActive ?? true
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
                null,
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
                TotalStock = p.TotalStock,
                IsActive = p.IsActive,

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
                            OptionId = o.OptionId,
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
            // 1️⃣ Lấy product + quan hệ cấp 1
            var product = await _unitOfWork.ProductRepository.GetAsync(
                p => p.ProductId == productId,
                p => p.Category,
                p => p.Brand,
                p => p.Images,
                p => p.Variations
            );

            if (product == null) return null;

            // 2️⃣ Lấy variationIds
            var variationIds = product.Variations
                .Select(v => v.VariationId)
                .ToList();

            // 3️⃣ Lấy options
            var options = await _unitOfWork.VariationOptionRepository
                .GetAllAsync(o => variationIds.Contains(o.VariationId));

            // 4️⃣ Lấy attributes
            var attributes = await _unitOfWork.VariationAttributeRepository
                .GetAllAsync();

            // 5️⃣ Lấy specifications
            var specs = await _unitOfWork.ProductSpecificationRepository
                .GetAllAsync(s => s.ProductId == product.ProductId);

            // 6️⃣ Map response
            return new ProductResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                CategoryName = product.Category?.CategoryName,
                CategoryId = product.Category?.CategoryId,
                BrandId = product.Brand?.BrandId,
                BrandName = product.Brand?.BrandName,
                ProductPrice = product.Variations.Any()
                    ? product.Variations.Min(v => v.Price)
                    : 0,
                TotalStock = product.TotalStock,
                IsActive = product.IsActive,

                Images = product.Images.Select(i => new ProductImageResponse
                {
                    ImageId = i.ImageId,
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain
                }).ToList(),

                Variations = product.Variations.Select(v => new ProductVariationResponse
                {
                    VariationId = v.VariationId,
                    Price = v.Price,
                    StockQuantity = v.StockQuantity,
                    Options = options
                        .Where(o => o.VariationId == v.VariationId)
                        .Select(o => new VariationOptionResponse
                        {
                            OptionId = o.OptionId,
                            AttributeId = o.AttributeId,
                            AttributeName = attributes
                                .FirstOrDefault(a => a.AttributeId == o.AttributeId)?.Name ?? "",
                            Value = o.Value
                        })
                        .ToList()
                }).ToList(),

                Specifications = specs.Select(s => new ProductSpecificationResponse
                {
                    SpecificationId = s.SpecificationId,
                    SpecKey = s.SpecKey,
                    SpecValue = s.SpecValue
                }).ToList()
            };
        }




        // ========================= UPDATE =========================
        public async Task<bool> UpdateProductAsync(UpdateProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ProductId))
                return false;

            // Allow updating product even when it is currently inactive so IsActive can be toggled
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == request.ProductId);

            if (product == null) return false;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                /* ================= PRODUCT ================= */
                bool productChanged = false;

                if (request.ProductName != null)
                {
                    product.ProductName = request.ProductName;
                    productChanged = true;
                }

                if (request.ProductDescription != null)
                {
                    product.ProductDescription = request.ProductDescription;
                    productChanged = true;
                }

                if (request.CategoryId != null)
                {
                    product.CategoryId = request.CategoryId;
                    productChanged = true;
                }

                if (request.BrandId != null)
                {
                    product.BrandId = request.BrandId;
                    productChanged = true;
                }

                // Toggle IsActive if specified in request
                if (request.IsActive != null && product.IsActive != request.IsActive.Value)
                {
                    product.IsActive = request.IsActive.Value;
                    productChanged = true;
                }

                if (productChanged)
                {
                    product.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);
                    _unitOfWork.ProductRepository.Update(product);
                }

                /* ================= VARIATIONS ================= */
                if (request.Variations != null && request.Variations.Any())
                {
                    // Precompute last ids once to avoid duplicates generated by repeated DB reads
                    var lastVariationId = await GenerateIdAsync(
                        u => u.ProductVariationRepository,
                        x => x.VariationId,
                        Prefixes.VARIATION_ID_PREFIX);
                    var variationIndex = int.Parse(lastVariationId.Substring(Prefixes.VARIATION_ID_PREFIX.Length));

                    var lastOptionId = await GenerateIdAsync(
                        u => u.VariationOptionRepository,
                        x => x.OptionId,
                        Prefixes.OPTION_ID_PREFIX);
                    var optionIndex = int.Parse(lastOptionId.Substring(Prefixes.OPTION_ID_PREFIX.Length));

                    foreach (var v in request.Variations)
                    {
                        /* ========== DELETE VARIATION ========== */
                        if (v.IsDeleted && !string.IsNullOrWhiteSpace(v.VariationId))
                        {
                            var delVar = await _unitOfWork.ProductVariationRepository
                                .GetAsync(x => x.VariationId == v.VariationId);

                            if (delVar != null)
                            {
                                var opts = await _unitOfWork.VariationOptionRepository
                                    .GetAllAsync(o => o.VariationId == delVar.VariationId);

                                foreach (var opt in opts)
                                    _unitOfWork.VariationOptionRepository.Delete(opt);

                                _unitOfWork.ProductVariationRepository.Delete(delVar);
                            }

                            continue;
                        }

                        /* ========== ADD / UPDATE VARIATION ========== */
                        ProductVariation ev;
                        bool isNewVariation = string.IsNullOrWhiteSpace(v.VariationId);

                        if (isNewVariation)
                        {
                            variationIndex++;
                            var newVariationId = Prefixes.VARIATION_ID_PREFIX + string.Format(Prefixes.ID_FORMAT, variationIndex);

                            ev = new ProductVariation
                            {
                                VariationId = newVariationId,
                                ProductId = product.ProductId,
                                Price = v.Price,
                                StockQuantity = v.StockQuantity
                            };

                            await _unitOfWork.ProductVariationRepository.AddAsync(ev);
                        }
                        else
                        {
                            ev = await _unitOfWork.ProductVariationRepository
                                .GetAsync(x => x.VariationId == v.VariationId);

                            if (ev == null) continue;

                            ev.Price = v.Price;
                            ev.StockQuantity = v.StockQuantity;
                            _unitOfWork.ProductVariationRepository.Update(ev);
                        }

                        /* ========== OPTIONS ========== */
                        foreach (var o in v.Options ?? Enumerable.Empty<UpdateVariationOptionRequest>())
                        {
                            /* ===== DELETE OPTION ===== */
                            if (o.IsDeleted && !string.IsNullOrWhiteSpace(o.OptionId))
                            {
                                var delOpt = await _unitOfWork.VariationOptionRepository
                                    .GetAsync(x => x.OptionId == o.OptionId);

                                if (delOpt != null)
                                    _unitOfWork.VariationOptionRepository.Delete(delOpt);

                                continue;
                            }

                            /* ===== ADD OPTION ===== */
                            if (string.IsNullOrWhiteSpace(o.OptionId))
                            {
                                optionIndex++;
                                var newOptionId = Prefixes.OPTION_ID_PREFIX + string.Format(Prefixes.ID_FORMAT, optionIndex);

                                await _unitOfWork.VariationOptionRepository.AddAsync(
                                    new VariationOption
                                    {
                                        OptionId = newOptionId,
                                        VariationId = ev.VariationId,
                                        AttributeId = o.AttributeId,
                                        Value = o.OptionValue
                                    });

                                continue;
                            }

                            /* ===== UPDATE OPTION ===== */
                            var eo = await _unitOfWork.VariationOptionRepository
                                .GetAsync(x => x.OptionId == o.OptionId);

                            if (eo != null)
                            {
                                eo.AttributeId = o.AttributeId;
                                eo.Value = o.OptionValue;
                                _unitOfWork.VariationOptionRepository.Update(eo);
                            }
                        }
                    }
                }

                /* ================= SPECIFICATIONS ================= */
                if (request.Specifications != null && request.Specifications.Any())
                {
                    var existingSpecs = (await _unitOfWork.ProductSpecificationRepository
                        .GetAllAsync(s => s.ProductId == product.ProductId))
                        .ToList();

                    foreach (var s in request.Specifications)
                    {
                        if (s.IsDeleted && s.SpecificationId != null)
                        {
                            var del = existingSpecs
                                .FirstOrDefault(x => x.SpecificationId == s.SpecificationId);

                            if (del != null)
                                _unitOfWork.ProductSpecificationRepository.Delete(del);

                            continue;
                        }

                        if (s.SpecificationId == null)
                        {
                            await _unitOfWork.ProductSpecificationRepository.AddAsync(
                                new ProductSpecification
                                {
                                    SpecificationId = await GenerateIdAsync(
                                        u => u.ProductSpecificationRepository,
                                        x => x.SpecificationId,
                                        Prefixes.SPECIFICATION_ID_PREFIX),
                                    ProductId = product.ProductId,
                                    SpecKey = s.SpecKey,
                                    SpecValue = s.SpecValue
                                });
                            continue;
                        }

                        var es = existingSpecs
                            .FirstOrDefault(x => x.SpecificationId == s.SpecificationId);

                        if (es != null)
                        {
                            es.SpecKey = s.SpecKey;
                            es.SpecValue = s.SpecValue;
                            _unitOfWork.ProductSpecificationRepository.Update(es);
                        }
                    }
                }

                /* ================= IMAGES ================= */
                if (request.Images != null && request.Images.Any())
                {
                    foreach (var img in request.Images)
                    {
                        if (img.IsDeleted && img.ImageId != null)
                        {
                            var del = await _unitOfWork.ProductImageRepository
                                .GetAsync(i => i.ImageId == img.ImageId);

                            if (del != null)
                                _unitOfWork.ProductImageRepository.Delete(del);

                            continue;
                        }

                        if (img.ImageId == null)
                        {
                            await _unitOfWork.ProductImageRepository.AddAsync(
                                new ProductImage
                                {
                                    ImageId = await GenerateIdAsync(
                                        u => u.ProductImageRepository,
                                        x => x.ImageId,
                                        Prefixes.IMAGE_ID_PREFIX),
                                    ProductId = product.ProductId,
                                    ImageUrl = img.ImageUrl,
                                    IsMain = img.IsMain
                                });
                            continue;
                        }

                        var ei = await _unitOfWork.ProductImageRepository
                            .GetAsync(i => i.ImageId == img.ImageId);

                        if (ei != null)
                        {
                            ei.ImageUrl = img.ImageUrl;
                            ei.IsMain = img.IsMain;
                            _unitOfWork.ProductImageRepository.Update(ei);
                        }
                    }
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
        //search
        public async Task<IEnumerable<ProductResponse>> SearchAsync(ProductSearchRequest request)
        {
            var keyword = request.Keyword?.Trim();
            var status = string.IsNullOrWhiteSpace(request.Status)
                ? "all"
                : request.Status.ToLower();

            var categoryIds = request.CategoryIds ?? new List<string>();
            var brandIds = request.BrandIds ?? new List<string>();

            var products = await _unitOfWork.ProductRepository.GetAllAsync(
                p =>
                    // 🔎 keyword
                    (string.IsNullOrEmpty(keyword) ||
                     EF.Functions.Like(p.ProductName, $"%{keyword}%"))

                    // 📂 multi-category
                    && (categoryIds.Count == 0 || categoryIds.Contains(p.CategoryId))

                    // 🏷 multi-brand
                    && (brandIds.Count == 0 || brandIds.Contains(p.BrandId))

                    // 🚦 status
                    && (
                        status == "all" ||
                        (status == "active" && p.IsActive) ||
                        (status == "inactive" && !p.IsActive)
                    ),

                p => p.Category,
                p => p.Brand,
                p => p.Images,
                p => p.Variations
            );

            // ===== map giống GetAllAsync =====
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
                ProductPrice = p.Variations.Any()
                    ? p.Variations.Min(v => v.Price)
                    : 0,
                TotalStock = p.TotalStock,
                IsActive = p.IsActive,

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
                            OptionId = o.OptionId,
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

    }
}
