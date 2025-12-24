using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Models;
using Domain.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/product-specifications")]
    public class ProductSpecificationController : ControllerBase
    {
        private readonly IProductSpecificationService _service;

        public ProductSpecificationController(IProductSpecificationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductSpecificationRequest request)
        {
            await _service.CreateAsync(request);
            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Create specification successfully"
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateProductSpecificationRequest request)
        {
            var ok = await _service.UpdateAsync(request);
            if (!ok)
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Specification not found"
                });

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Update specification successfully"
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
                    Message = "Specification not found"
                });

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Delete specification successfully"
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
