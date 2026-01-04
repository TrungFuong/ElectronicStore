using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var result = await _productService.CreateProductAsync(request);

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Create product successfully"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _productService.GetAllAsync();

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get products successfully",
                Data = data
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Data = product
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductRequest request)
        {
            request.ProductId = id;
            var ok = await _productService.UpdateProductAsync(request);

            if (!ok)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Update product successfully"
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _productService.DeleteProductAsync(id);

            if (!ok)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Delete product successfully"
            });
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(string categoryId)
        {
            var products = await _productService.GetByCategoryAsync(categoryId);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get products by category successfully",
                Data = products
            });
        }

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetByBrand(string brandId)
        {
            var products = await _productService.GetByBrandAsync(brandId);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get products by brand successfully",
                Data = products
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] ProductSearchRequest request)
        {
            var data = await _productService.SearchAsync(request);
            return Ok(new
            {
                success = true,
                data
            });
        }
    }
}
