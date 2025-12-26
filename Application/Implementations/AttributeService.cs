using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class AttributeService : IAttributeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttributeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AttributeResponse>> GetAllAsync()
        {
            var attributes = await _unitOfWork.VariationAttributeRepository.GetAllAsync();

            return attributes.Select(a => new AttributeResponse
            {
                AttributeId = a.AttributeId,
                Name = a.Name
            });
        }


    }
}
