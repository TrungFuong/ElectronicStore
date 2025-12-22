using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DiscountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // CREATE
        public async Task<bool> CreateDiscountAsync(CreateDiscountRequest request)
        {
            var count = await _unitOfWork.DiscountRepository.CountAsync();

            var discount = new Discount
            {
                DiscountId = Prefixes.DISCOUNT_ID_PREFIX + string.Format(Prefixes.ID_FORMAT, count + 1),
                DiscountName = request.DiscountName,
                DiscountCode = request.DiscountCode,
                DiscountDescription = request.DiscountDescription,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                MinOrderValue = request.MinOrderValue,
                MaxDiscountAmount = request.MaxDiscountAmount,
                StartDate = request.StartDate,
                ExpireDate = request.ExpireDate,
                UsageLimit = request.UsageLimit,
                UsageCount = 0,
                IsActive = request.IsActive
            };

            await _unitOfWork.DiscountRepository.AddAsync(discount);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // READ
        public async Task<IEnumerable<DiscountResponse>> GetAllAsync()
        {
            var discounts = await _unitOfWork.DiscountRepository.GetAllAsync();

            return discounts.Select(d => new DiscountResponse
            {
                DiscountId = d.DiscountId,
                DiscountName = d.DiscountName,
                DiscountCode = d.DiscountCode,
                DiscountDescription = d.DiscountDescription,
                DiscountType = (int)d.DiscountType,
                DiscountValue = d.DiscountValue,
                MinOrderValue = d.MinOrderValue,
                MaxDiscountAmount = d.MaxDiscountAmount,
                StartDate = d.StartDate,
                ExpireDate = d.ExpireDate,
                UsageLimit = d.UsageLimit,
                UsageCount = d.UsageCount,
                IsActive = d.IsActive
            });
        }

        public async Task<DiscountResponse?> GetByIdAsync(string discountId)
        {
            var d = await _unitOfWork.DiscountRepository.GetAsync(x => x.DiscountId == discountId);
            if (d == null) return null;

            return new DiscountResponse
            {
                DiscountId = d.DiscountId,
                DiscountName = d.DiscountName,
                DiscountCode = d.DiscountCode,
                DiscountDescription = d.DiscountDescription,
                DiscountType = (int)d.DiscountType,
                DiscountValue = d.DiscountValue,
                MinOrderValue = d.MinOrderValue,
                MaxDiscountAmount = d.MaxDiscountAmount,
                StartDate = d.StartDate,
                ExpireDate = d.ExpireDate,
                UsageLimit = d.UsageLimit,
                UsageCount = d.UsageCount,
                IsActive = d.IsActive
            };
        }

        // UPDATE
        public async Task<bool> UpdateDiscountAsync(UpdateDiscountRequest request)
        {
            var discount = await _unitOfWork.DiscountRepository.GetAsync(d => d.DiscountId == request.DiscountId);
            if (discount == null) return false;

            discount.DiscountName = request.DiscountName;
            discount.DiscountCode = request.DiscountCode;
            discount.DiscountDescription = request.DiscountDescription;
            discount.DiscountType = request.DiscountType;
            discount.DiscountValue = request.DiscountValue;
            discount.MinOrderValue = request.MinOrderValue;
            discount.MaxDiscountAmount = request.MaxDiscountAmount;
            discount.StartDate = request.StartDate;
            discount.ExpireDate = request.ExpireDate;
            discount.UsageLimit = request.UsageLimit;
            discount.IsActive = request.IsActive;

            _unitOfWork.DiscountRepository.Update(discount);
            await _unitOfWork.CommitAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteDiscountAsync(string discountId)
        {
            var discount = await _unitOfWork.DiscountRepository.GetAsync(d => d.DiscountId == discountId);
            if (discount == null) return false;

            _unitOfWork.DiscountRepository.SoftDelete(discount); // soft delete
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
