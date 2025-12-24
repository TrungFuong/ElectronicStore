using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Models;
using Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/product-variations")]
    public class ProductVariationController : ControllerBase
    {
        private readonly IProductVariationService _service;

        public ProductVariationController(IProductVariationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVariationRequest request)
        {
            await _service.CreateAsync(request);
            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Create variation successfully"
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductVariationRequest request)
        {
            var ok = await _service.UpdateAsync(request);
            if (!ok)
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Variation not found"
                });

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Update variation successfully"
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
                    Message = "Variation not found"
                });

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Delete variation successfully"
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var data = await _service.GetByIdAsync(id);
            if (data == null)
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Variation not found"
                });

            return Ok(new GeneralGetResponse { Data = data });
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(string productId)
        {
            var data = await _service.GetByProductIdAsync(productId);
            return Ok(new GeneralGetResponse { Data = data });
        }
    }
}
