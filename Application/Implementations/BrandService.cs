using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // CREATE
        public async Task CreateAsync(CreateBrandRequest request)
        {
            var newId = await _unitOfWork.BrandRepository.GenerateNewBrandIdAsync();

            var brand = new Brand
            {
                BrandId = newId,
                BrandName = request.BrandName,
                BrandDescription = request.BrandDescription
            };

            await _unitOfWork.BrandRepository.AddAsync(brand);
            await _unitOfWork.CommitAsync();
        }

        // READ
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

        // UPDATE
        public async Task<bool> UpdateAsync(UpdateBrandRequest request)
        {
            var brand = await _unitOfWork.BrandRepository.GetAsync(b => b.BrandId == request.BrandId);
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
            var brand = await _unitOfWork.BrandRepository.GetAsync(b => b.BrandId == request.BrandId);
            if (brand == null) return false;

            _unitOfWork.BrandRepository.Delete(brand); // hard delete
            await _unitOfWork.CommitAsync();
            return true;
        }


    }
}