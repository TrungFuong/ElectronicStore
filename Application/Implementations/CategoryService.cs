using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // PRIVATE: GENERATE CATEGORY ID 
        private async Task<string> GenerateCategoryIdAsync()
        {
            var lastCategory = (await _unitOfWork.CategoryRepository.GetAllAsync())
                .OrderByDescending(c => c.CategoryId)
                .FirstOrDefault();

            if (lastCategory == null)
                return Prefixes.CATEGORY_ID_PREFIX + "0001";

            var numberPart = lastCategory.CategoryId
                .Substring(Prefixes.CATEGORY_ID_PREFIX.Length);

            var nextNumber = int.Parse(numberPart) + 1;

            return Prefixes.CATEGORY_ID_PREFIX + nextNumber.ToString("D4");
        }

        // CREATE 
        public async Task CreateAsync(CreateCategoryRequest request)
        {
            var category = new Category
            {
                CategoryId = await GenerateCategoryIdAsync(),
                CategoryName = request.CategoryName,
                CategoryDescription = request.CategoryDescription,
                IsActive = true
            };

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.CommitAsync();
        }

        //READ
        public async Task<IEnumerable<CategoryResponse>> GetAllAsync()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();

            return categories.Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDescription = c.CategoryDescription
            });
        }

        //UPDATE
        public async Task<bool> UpdateAsync(UpdateCategoryRequest request)
        {
            var category = await _unitOfWork.CategoryRepository
                .GetAsync(c => c.CategoryId == request.CategoryId && c.IsActive);

            if (category == null) return false;

            category.CategoryName = request.CategoryName;
            category.CategoryDescription = request.CategoryDescription;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.CommitAsync();

            return true;
        }

        // DELETE
        public async Task<bool> DeleteAsync(DeleteCategoryRequest request)
        {
            var category = await _unitOfWork.CategoryRepository
                .GetAsync(c => c.CategoryId == request.CategoryId && c.IsActive);

            if (category == null) return false;

            _unitOfWork.CategoryRepository.SoftDelete(category);
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}
