using Application.DTOs.Auth;
using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


//  NO AUTHORIZATION 


namespace API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // POST api/categories
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            try
            {
                await _categoryService.CreateAsync(request);
                return Ok(new GeneralBoolResponse
                {
                    Message = "Tạo category thành công"
                });
            }
            catch (Exception ex)
            {
                return Conflict(new GeneralBoolResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        // GET api/categories
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _categoryService.GetAllAsync();
            return Ok(new GeneralGetResponse
            {
                Message = "Lấy danh sách category thành công",
                Data = data
            });
        }

        // PUT api/categories
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCategoryRequest request)
        {
            var success = await _categoryService.UpdateAsync(request);
            if (!success)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Category không tồn tại"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Message = "Cập nhật category thành công"
            });
        }

        // DELETE api/categories
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteCategoryRequest request)
        {
            var success = await _categoryService.DeleteAsync(request);
            if (!success)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Category không tồn tại"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Message = "Xóa category thành công"
            });
        }

    }
}