using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Models;
using Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;

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

        // CREATE 
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductRequest request)
        {
            var ok = await _productService.CreateProductAsync(request);
            return Ok(new GeneralBoolResponse
            {
                Success = ok,
                Message = "Create product successfully"
            });
        }

        //  GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _productService.GetAllAsync();
            return Ok(new GeneralGetResponse
            {
                Data = data,
                Message = "Get products successfully"
            });
        }

        // GET BY ID 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Product not found"
                });

            return Ok(new GeneralGetResponse { Data = product });
        }

        // UPDATE
        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductRequest request)
        {
            var ok = await _productService.UpdateProductAsync(request);
            if (!ok)
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Product not found"
                });

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Update product successfully"
            });
        }

        // DELETE 
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _productService.DeleteProductAsync(id);
            if (!ok)
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Product not found"
                });

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Delete product successfully"
            });
        }

        //BY CATEGORY 
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(string categoryId)
        {
            var products = await _productService.GetByCategoryAsync(categoryId);
            return Ok(new GeneralGetResponse
            {
                Data = products,
                Message = "Get products by category successfully"
            });
        }

        // BY BRAND 
        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetByBrand(string brandId)
        {
            var products = await _productService.GetByBrandAsync(brandId);

            return Ok(new GeneralGetResponse
            {
                Data = products,
                Message = "Get products by brand successfully"
            });
        }
    }
}
