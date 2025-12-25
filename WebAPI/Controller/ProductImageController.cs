using Application.DTOs.Requests;
using Application.Interfaces;
using Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/images")]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _service;

        public ProductImageController(IProductImageService service)
        {
            _service = service;
        }

        // =========================
        // CREATE IMAGES
        // POST /api/products/{productId}/images
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(
            string productId,
            [FromBody] CreateProductImageRequest request)
        {
            request.ProductId = productId; // 🔥 lấy từ route

            await _service.CreateAsync(request);

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Create image successfully"
            });
        }

        // =========================
        // DELETE IMAGE
        // DELETE /api/products/{productId}/images/{imageId}
        // =========================
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> Delete(
            string productId,
            string imageId)
        {
            var ok = await _service.DeleteAsync(imageId);

            if (!ok)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Image not found"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Delete image successfully"
            });
        }

        // =========================
        // GET IMAGES BY PRODUCT
        // GET /api/products/{productId}/images
        // =========================
        [HttpGet]
        public async Task<IActionResult> GetByProduct(string productId)
        {
            var data = await _service.GetByProductIdAsync(productId);

            return Ok(new GeneralGetResponse
            {
                Data = data
            });
        }
    }
}
