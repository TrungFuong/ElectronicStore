using Application.DTOs.Auth;
using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // ============================
        // GET: api/product
        // Public
        // ============================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _productService.GetAllAsync();
                return Ok(new GeneralGetResponse
                {
                    Success = true,
                    Message = "Lấy danh sách sản phẩm thành công",
                    Data = result
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

        // ============================
        // GET: api/product/{id}
        // Public
        // ============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var result = await _productService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new GeneralBoolResponse
                    {
                        Success = false,
                        Message = "Không tìm thấy sản phẩm"
                    });
                }

                return Ok(new GeneralGetResponse
                {
                    Success = true,
                    Data = result
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

        // ============================
        // POST: api/product
        // ADMIN, STAFF
        // ============================
        [HttpPost]
        
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            try
            {
                var result = await _productService.CreateProductAsync(request);
                return Ok(new GeneralBoolResponse
                {
                    Success = result,
                    Message = "Tạo sản phẩm thành công"
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

        // ============================
        // PUT: api/product
        // ADMIN, STAFF
        // ============================
        [HttpPut]
        [Authorize(Roles = "ADMIN,STAFF")]
        public async Task<IActionResult> Update([FromBody] UpdateProductRequest request)
        {
            try
            {
                var result = await _productService.UpdateProductAsync(request);
                return Ok(new GeneralBoolResponse
                {
                    Success = result,
                    Message = "Cập nhật sản phẩm thành công"
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

        // ============================
        // DELETE: api/product/{id}
        // ADMIN
        // ============================
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                return Ok(new GeneralBoolResponse
                {
                    Success = result,
                    Message = "Xóa sản phẩm thành công"
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
    }
}
