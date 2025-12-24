using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductSpecificationService : IProductSpecificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductSpecificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // CREATE 
        public async Task<bool> CreateAsync(CreateProductSpecificationRequest request)
        {
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == request.ProductId && p.IsActive);

            if (product == null)
                throw new Exception("Product không tồn tại");

            if (request.Specifications == null || !request.Specifications.Any())
                throw new Exception("Danh sách specification rỗng");

            var duplicateKeys = request.Specifications
                .GroupBy(s => s.SpecKey.Trim())
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateKeys.Any())
                throw new Exception($"SpecKey bị trùng: {string.Join(", ", duplicateKeys)}");

            var count = await _unitOfWork.ProductSpecificationRepository.CountAsync();

            var specs = request.Specifications.Select((s, index) =>
                new ProductSpecification
                {
                    SpecificationId = Prefixes.SPECIFICATION_ID_PREFIX
                        + string.Format(Prefixes.ID_FORMAT, count + index + 1),
                    ProductId = request.ProductId,
                    SpecKey = s.SpecKey.Trim(),
                    SpecValue = s.SpecValue.Trim()
                }).ToList();

            await _unitOfWork.ProductSpecificationRepository.AddRangeAsync(specs);
            await _unitOfWork.CommitAsync();

            return true;
        }

        // UPDATE 
        public async Task<bool> UpdateAsync(UpdateProductSpecificationRequest request)
        {
            var spec = await _unitOfWork.ProductSpecificationRepository
                .GetAsync(s => s.SpecificationId == request.SpecificationId);

            if (spec == null) return false;

            if (string.IsNullOrWhiteSpace(request.SpecValue))
                throw new Exception("SpecValue không được để trống");

            spec.SpecValue = request.SpecValue;

            _unitOfWork.ProductSpecificationRepository.Update(spec);
            await _unitOfWork.CommitAsync();
            return true;
        }

        //DELETE 
        public async Task<bool> DeleteAsync(string specificationId)
        {
            var spec = await _unitOfWork.ProductSpecificationRepository
                .GetAsync(s => s.SpecificationId == specificationId);

            if (spec == null) return false;

            _unitOfWork.ProductSpecificationRepository.Delete(spec);
            await _unitOfWork.CommitAsync();
            return true;
        }

        //GET BY PRODUCT 
        public async Task<IEnumerable<ProductSpecificationResponse>>
            GetByProductIdAsync(string productId)
        {
            var specs = await _unitOfWork.ProductSpecificationRepository
                .GetAllAsync(s => s.ProductId == productId);

            return specs.Select(s => new ProductSpecificationResponse
            {
                SpecificationId = s.SpecificationId,
                SpecKey = s.SpecKey,
                SpecValue = s.SpecValue
            });
        }
    }
}
