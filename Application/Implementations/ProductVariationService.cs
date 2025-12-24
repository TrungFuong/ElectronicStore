using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Implementations
{
    public class ProductVariationService : IProductVariationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductVariationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        private async Task<string> GenerateNextIdAsync(
            Func<Task<IEnumerable<string>>> getAllIds,
            string prefix)
                {
                    var ids = await getAllIds();

                    var lastId = ids
                        .Where(id => id.StartsWith(prefix))
                        .OrderByDescending(id => id)
                        .FirstOrDefault();

                    if (lastId == null)
                        return prefix + "0001";

                    var numberPart = lastId.Substring(prefix.Length);
                    var nextNumber = int.Parse(numberPart) + 1;

                    return prefix + nextNumber.ToString("D4");
                }
        //CREATE 
        public async Task<bool> CreateAsync(CreateProductVariationRequest request)
        {
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == request.ProductId && p.IsActive);

            if (product == null)
                throw new Exception("Product không tồn tại");

            // VARIATION ID
            var variationId = await GenerateNextIdAsync(
                async () => (await _unitOfWork.ProductVariationRepository.GetAllAsync())
                            .Select(v => v.VariationId),
                Prefixes.VARIATION_ID_PREFIX
            );

            var variation = new ProductVariation
            {
                VariationId = variationId,
                ProductId = request.ProductId,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                IsActive = true,
                Options = new List<VariationOption>()
            };

            // OPTION ID
            if (request.Options != null)
            {
                var existingOptionIds = await _unitOfWork.VariationOptionRepository.GetAllAsync();

                var currentMax = existingOptionIds
                    .Select(o => o.OptionId)
                    .Where(id => id.StartsWith(Prefixes.OPTION_ID_PREFIX))
                    .OrderByDescending(id => id)
                    .FirstOrDefault();

                int optionCounter = currentMax == null
                    ? 1
                    : int.Parse(currentMax.Substring(Prefixes.OPTION_ID_PREFIX.Length)) + 1;

                foreach (var o in request.Options)
                {
                    variation.Options.Add(new VariationOption
                    {
                        OptionId = Prefixes.OPTION_ID_PREFIX + optionCounter.ToString("D4"),
                        AttributeId = o.AttributeId,
                        Value = o.Value
                    });

                    optionCounter++;
                }
            }

            await _unitOfWork.ProductVariationRepository.AddAsync(variation);
            await _unitOfWork.CommitAsync();

            return true;
        }


        // UPDATE 
        public async Task<bool> UpdateAsync(UpdateProductVariationRequest request)
        {
            var variation = await _unitOfWork.ProductVariationRepository
                .GetAsync(v => v.VariationId == request.VariationId,
                          v => v.Options);

            if (variation == null) return false;

            variation.Price = request.Price;
            variation.StockQuantity = request.StockQuantity;

            variation.Options.Clear();

            if (request.Options != null)
            {
                foreach (var o in request.Options)
                {
                    variation.Options.Add(new VariationOption
                    {
                        AttributeId = o.AttributeId,
                        Value = o.Value
                    });
                }
            }

            _unitOfWork.ProductVariationRepository.Update(variation);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // DELETE 
        public async Task<bool> DeleteAsync(string variationId)
        {
            var variation = await _unitOfWork.ProductVariationRepository
                .GetAsync(v => v.VariationId == variationId);

            if (variation == null) return false;

            _unitOfWork.ProductVariationRepository.SoftDelete(variation);
            await _unitOfWork.CommitAsync();
            return true;
        }

        //GET BY ID 
        public async Task<ProductVariationResponse?> GetByIdAsync(string variationId)
        {
            var variation = await _unitOfWork.ProductVariationRepository
                .GetAsync(v => v.VariationId == variationId,
                          v => v.Options);

            if (variation == null) return null;

            // Load attributes riêng
            var attributes = await _unitOfWork.VariationAttributeRepository.GetAllAsync();

            return new ProductVariationResponse
            {
                VariationId = variation.VariationId,
                Price = variation.Price,
                StockQuantity = variation.StockQuantity,
                Options = variation.Options.Select(o => new VariationOptionResponse
                {
                    AttributeId = o.AttributeId,
                    AttributeName = attributes
                        .FirstOrDefault(a => a.AttributeId == o.AttributeId)?.Name ?? "",
                    Value = o.Value
                }).ToList()
            };
        }

        //GET BY PRODUCT 
        public async Task<IEnumerable<ProductVariationResponse>>
            GetByProductIdAsync(string productId)
        {
            var variations = await _unitOfWork.ProductVariationRepository
                .GetAllAsync(v => v.ProductId == productId,
                             v => v.Options);

            // Load attributes 1 lần
            var attributes = await _unitOfWork.VariationAttributeRepository
                .GetAllAsync();

            return variations.Select(v => new ProductVariationResponse
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
            });
        }
    }
}
