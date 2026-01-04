using Application.DTOs.Requests;
using Application.Interfaces;
using Application.DTOs.Responses;
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [HttpPatch("status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetStatus([FromBody] ToggleCategoryStatusRequest request)
        {
            var ok = await _categoryService.SetActiveAsync(request.CategoryId, request.IsActive);
            if (!ok)
                return NotFound(new GeneralBoolResponse { Success = false, Message = "Category không tồn tại" });

            return Ok(new GeneralBoolResponse { Success = true, Message = "Cập nhật trạng thái thành công" });
        }

        // DELETE api/categories
        [HttpDelete]
        [Authorize(Roles = "Admin")]
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