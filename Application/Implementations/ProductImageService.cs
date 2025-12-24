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
        private async Task<string> GenerateImageIdAsync()
        {
            var lastImage = (await _unitOfWork.ProductImageRepository.GetAllAsync())
                .OrderByDescending(i => i.ImageId)
                .FirstOrDefault();

            if (lastImage == null)
                return Prefixes.IMAGE_ID_PREFIX + "0001";

            var numberPart = lastImage.ImageId
                .Substring(Prefixes.IMAGE_ID_PREFIX.Length);

            var nextNumber = int.Parse(numberPart) + 1;

            return Prefixes.IMAGE_ID_PREFIX + nextNumber.ToString("D4");
        }


        //  CREATE 
        public async Task<bool> CreateAsync(CreateProductImageRequest request)
        {
            // 1. Check product
            var product = await _unitOfWork.ProductRepository
                .GetAsync(p => p.ProductId == request.ProductId && p.IsActive);

            if (product == null)
                throw new Exception("Product không tồn tại");

            if (request.Images == null || !request.Images.Any())
                throw new Exception("Danh sách ảnh rỗng");

            // 2. Max 4 ảnh
            if (request.Images.Count > 4)
                throw new Exception("Tối đa 4 ảnh cho mỗi sản phẩm");

            // 3. Phải có đúng 1 ảnh main
            var mainCount = request.Images.Count(i => i.IsMain);
            if (mainCount != 1)
                throw new Exception("Phải có đúng 1 ảnh main");

            // 4. Validate ImageUrl
            if (request.Images.Any(i => string.IsNullOrWhiteSpace(i.ImageUrl)))
                throw new Exception("ImageUrl không được để trống");

            // 5. Bỏ ảnh main cũ (nếu có)
            var oldMainImages = await _unitOfWork.ProductImageRepository
                .GetAllAsync(i => i.ProductId == request.ProductId && i.IsMain);

            foreach (var img in oldMainImages)
            {
                img.IsMain = false;
                _unitOfWork.ProductImageRepository.Update(img);
            }

            var startId = await GenerateImageIdAsync();
            var startNumber = int.Parse(
                startId.Substring(Prefixes.IMAGE_ID_PREFIX.Length)
            );
            // 6. Tạo entity
            var images = request.Images.Select((i, index) => new ProductImage
            {
                ImageId = Prefixes.IMAGE_ID_PREFIX
              + (startNumber + index).ToString("D4"),
                ProductId = request.ProductId,
                ImageUrl = i.ImageUrl,
                IsMain = i.IsMain
            }).ToList();

            // 7. Add + commit 1 lần
            await _unitOfWork.ProductImageRepository.AddRangeAsync(images);
            await _unitOfWork.CommitAsync();

            return true;
        }

        
        // DELETE
        
        public async Task<bool> DeleteAsync(string imageId)
        {
            var image = await _unitOfWork.ProductImageRepository
                .GetAsync(i => i.ImageId == imageId);

            if (image == null) return false;

            _unitOfWork.ProductImageRepository.SoftDelete(image);
            await _unitOfWork.CommitAsync();
            return true;
        }

        
        // GET BY PRODUCT
        public async Task<IEnumerable<ProductImageResponse>> GetByProductIdAsync(string productId)
        {
            var images = await _unitOfWork.ProductImageRepository
                .GetAllAsync(i => i.ProductId == productId);

            return images.Select(i => new ProductImageResponse
            {
                ImageId = i.ImageId,
                ImageUrl = i.ImageUrl,
                IsMain = i.IsMain
            });
        }
    }
}
