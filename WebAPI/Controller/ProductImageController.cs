using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Models;
using Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/product-images")]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _service;

        public ProductImageController(IProductImageService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductImageRequest request)
        {
            await _service.CreateAsync(request);
            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Create image successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok)
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Image not found"
                });

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Delete image successfully"
            });
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(string productId)
        {
            var data = await _service.GetByProductIdAsync(productId);
            return Ok(new GeneralGetResponse { Data = data });
        }
    }
}
