using Application.DTOs.Requests;
using Application.Interfaces;
using Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/products/{productId}/variations")]
    public class ProductVariationController : ControllerBase
    {
        private readonly IProductVariationService _service;

        public ProductVariationController(IProductVariationService service)
        {
            _service = service;
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(
            string productId,
            [FromBody] CreateProductVariationRequest request)
        {
            request.ProductId = productId; 

            await _service.CreateAsync(request);

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Create variation successfully"
            });
        }

        
        [HttpPut]
        public async Task<IActionResult> Update(
            string productId,
            [FromBody] UpdateProductVariationRequest request)
        {
            // productId để validate nếu muốn
            var ok = await _service.UpdateAsync(request);

            if (!ok)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Variation not found"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Update variation successfully"
            });
        }

        
        [HttpDelete("{variationId}")]
        public async Task<IActionResult> Delete(
            string productId,
            string variationId)
        {
            var ok = await _service.DeleteAsync(variationId);

            if (!ok)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Variation not found"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Delete variation successfully"
            });
        }

        
        [HttpGet("{variationId}")]
        public async Task<IActionResult> GetById(
            string productId,
            string variationId)
        {
            var data = await _service.GetByIdAsync(variationId);

            if (data == null)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Variation not found"
                });
            }

            return Ok(new GeneralGetResponse
            {
                Data = data
            });
        }

        
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
