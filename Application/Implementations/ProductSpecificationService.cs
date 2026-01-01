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

    }
}
