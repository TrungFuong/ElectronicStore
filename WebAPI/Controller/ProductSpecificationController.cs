using Application.DTOs.Requests;
using Application.Interfaces;
using Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/specifications")]
    public class ProductSpecificationController : ControllerBase
    {
        private readonly IProductSpecificationService _service;

        public ProductSpecificationController(IProductSpecificationService service)
        {
            _service = service;
        }

        // =========================
        // CREATE SPECIFICATIONS
        // POST /api/products/{productId}/specifications
        // =========================
        [HttpPost]
        public async Task<IActionResult> Create(
            string productId,
            [FromBody] CreateProductSpecificationRequest request)
        {
            request.ProductId = productId; // 🔥 lấy từ route

            await _service.CreateAsync(request);

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Create specification successfully"
            });
        }

        // =========================
        // UPDATE SPECIFICATION
        // PUT /api/products/{productId}/specifications
        // =========================
        [HttpPut]
        public async Task<IActionResult> Update(
            string productId,
            [FromBody] UpdateProductSpecificationRequest request)
        {
            var ok = await _service.UpdateAsync(request);

            if (!ok)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Specification not found"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Update specification successfully"
            });
        }

        // =========================
        // DELETE SPECIFICATION
        // DELETE /api/products/{productId}/specifications/{specificationId}
        // =========================
        [HttpDelete("{specificationId}")]
        public async Task<IActionResult> Delete(
            string productId,
            string specificationId)
        {
            var ok = await _service.DeleteAsync(specificationId);

            if (!ok)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Specification not found"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Delete specification successfully"
            });
        }

        // =========================
        // GET SPECIFICATIONS BY PRODUCT
        // GET /api/products/{productId}/specifications
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
