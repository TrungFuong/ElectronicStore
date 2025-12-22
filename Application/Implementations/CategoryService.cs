using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(CreateCategoryRequest request)
        {
            var count = await _unitOfWork.CategoryRepository.CountAsync();

            var category = new Category
            {
                CategoryId = Prefixes.CATEGORY_ID_PREFIX + string.Format(Prefixes.ID_FORMAT, count + 1),
                CategoryName = request.CategoryName,
                CategoryDescription = request.CategoryDescription,
                IsActive = true
            };

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            return categories.Select(b => new CategoryResponse
            {
                CategoryId = b.CategoryId,
                CategoryName = b.CategoryName,
                CategoryDescription = b.CategoryDescription
            });
        }

        public async Task<bool> UpdateAsync(UpdateCategoryRequest request)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(c => c.CategoryId == request.CategoryId && c.IsActive);

            if (category == null) return false;

            category.CategoryName = request.CategoryName;
            category.CategoryDescription = request.CategoryDescription;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.CommitAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(DeleteCategoryRequest request)
        {
            var category = await _unitOfWork.CategoryRepository.GetAsync(c => c.CategoryId == request.CategoryId && c.IsActive);

            if (category == null) return false;

            _unitOfWork.CategoryRepository.Delete(category);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
