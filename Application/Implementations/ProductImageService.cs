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
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductImageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
    }
}
