using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task CreateAsync(CreateCategoryRequest request);
        Task<bool> UpdateAsync(UpdateCategoryRequest request);
        Task<bool> DeleteAsync(DeleteCategoryRequest request);
        Task<IEnumerable<CategoryResponse>> GetAllAsync();
    }
}
