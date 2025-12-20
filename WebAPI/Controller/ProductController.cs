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
        public async Task<IActionResult> Create(CreateProductRequest request)
        {
            var ok = await _productService.CreateProductAsync(request);
            return Ok(new GeneralBoolResponse
            {
                Success = ok,
                Message = "Create product successfully"
            });
        }

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

            return Ok(new GeneralGetResponse
            {
                Data = product
            });
        }

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
                Message = "Update product successfully"
            });
        }

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
                Message = "Delete product successfully"
            });
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(string categoryId)
        {
            var products = await _productService.GetByCategoryAsync(categoryId);

            return Ok(new GeneralGetResponse
            {
                Data = products
            });
        }

    }
}
