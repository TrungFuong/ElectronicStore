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
        
    }
}
