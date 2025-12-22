using Application.Interfaces;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/discounts")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var discounts = await _discountService.GetAllAsync();
            return Ok(new GeneralGetResponse { Data = discounts });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var discount = await _discountService.GetByIdAsync(id);
            if (discount == null) return NotFound(new GeneralBoolResponse { Success = false, Message = "Discount not found" });
            return Ok(new GeneralGetResponse { Data = discount });
        }

        [HttpPost]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> Create([FromBody] CreateDiscountRequest request)
        {
            var created = await _discountService.CreateDiscountAsync(request);
            return Ok(new GeneralBoolResponse { Success = created, Message = "Discount created" });
        }

        [HttpPut]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> Update([FromBody] UpdateDiscountRequest request)
        {
            var ok = await _discountService.UpdateDiscountAsync(request);
            if (!ok) return NotFound(new GeneralBoolResponse { Success = false, Message = "Discount not found" });
            return Ok(new GeneralBoolResponse { Message = "Discount updated" });
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _discountService.DeleteDiscountAsync(id);
            if (!ok) return NotFound(new GeneralBoolResponse { Success = false, Message = "Discount not found" });
            return Ok(new GeneralBoolResponse { Message = "Discount deleted" });
        }
    }
}
