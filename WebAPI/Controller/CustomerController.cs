using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var id = await _customerService.CreateAsync(request, accountId);
            return Ok(new GeneralGetResponse { Data = new { CustomerId = id }, Message = "Customer created" });
        }

        [HttpGet]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _customerService.GetAllAsync();
            return Ok(new GeneralGetResponse { Data = data });
        }

        [Authorize]
        [HttpGet("by-account")]
        public async Task<IActionResult> GetByAccountId()
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return Unauthorized(new GeneralGetResponse
                {
                    Success = false,
                    Message = "Khong xac dinh duoc nguoi dung"
                });
            }

            var customer = await _customerService.GetByAccountIdAsync(accountId);

            if (customer == null)
            {
                return NotFound(new GeneralGetResponse
                {
                    Success = false,
                    Message = "Customer not found"
                });
            }

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Data = customer
            });
        }

        [HttpPut]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerRequest request)
        {
            var ok = await _customerService.UpdateAsync(request);
            if (!ok) return NotFound(new GeneralGetResponse { Success = false, Message = "Customer not found" });
            return Ok(new GeneralGetResponse { Message = "Customer updated" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _customerService.DeleteAsync(id);
            if (!ok) return NotFound(new GeneralGetResponse { Success = false, Message = "Customer not found" });
            return Ok(new GeneralGetResponse { Message = "Customer deleted" });
        }
    }
}
