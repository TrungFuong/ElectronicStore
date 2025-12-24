using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //PRIVATE: GENERATE ID 
        private async Task<string> GenerateBrandIdAsync()
        {
            var lastBrand = (await _unitOfWork.BrandRepository.GetAllAsync())
                .OrderByDescending(b => b.BrandId)
                .FirstOrDefault();

            if (lastBrand == null)
                return Prefixes.BRAND_ID_PREFIX + "0001";

            var numberPart = lastBrand.BrandId.Substring(Prefixes.BRAND_ID_PREFIX.Length);
            var nextNumber = int.Parse(numberPart) + 1;

            return Prefixes.BRAND_ID_PREFIX + nextNumber.ToString("D4");
        }

        // CREATE
        public async Task CreateAsync(CreateBrandRequest request)
        {
            var brand = new Brand
            {
                BrandId = await GenerateBrandIdAsync(),
                BrandName = request.BrandName,
                BrandDescription = request.BrandDescription
            };

            await _unitOfWork.BrandRepository.AddAsync(brand);
            await _unitOfWork.CommitAsync();
        }

        //READ
        public async Task<IEnumerable<BrandResponse>> GetAllAsync()
        {
            var brands = await _unitOfWork.BrandRepository.GetAllAsync();

            return brands.Select(b => new BrandResponse
            {
                BrandId = b.BrandId,
                BrandName = b.BrandName,
                BrandDescription = b.BrandDescription
            });
        }

        //UPDATE
        public async Task<bool> UpdateAsync(UpdateBrandRequest request)
        {
            var brand = await _unitOfWork.BrandRepository
                .GetAsync(b => b.BrandId == request.BrandId);

            if (brand == null) return false;

            brand.BrandName = request.BrandName;
            brand.BrandDescription = request.BrandDescription;

            _unitOfWork.BrandRepository.Update(brand);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(DeleteBrandRequest request)
        {
            var brand = await _unitOfWork.BrandRepository
                .GetAsync(b => b.BrandId == request.BrandId);

            if (brand == null) return false;

            _unitOfWork.BrandRepository.SoftDelete(brand);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
