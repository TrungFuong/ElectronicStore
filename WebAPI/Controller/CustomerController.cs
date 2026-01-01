using Application.DTOs.Requests;
using Application.Interfaces;
using Application.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            var id = await _customerService.CreateAsync(request);
            return Ok(new GeneralGetResponse { Data = new { CustomerId = id }, Message = "Customer created" });
        }

        [HttpGet]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _customerService.GetAllAsync();
            return Ok(new GeneralGetResponse { Data = data });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null) return NotFound(new GeneralGetResponse { Success = false, Message = "Customer not found" });
            return Ok(new GeneralGetResponse { Data = customer });
        }

        [HttpPut]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> Update([FromBody] UpdateCustomerRequest request)
        {
            var ok = await _customerService.UpdateAsync(request);
            if (!ok) return NotFound(new GeneralGetResponse { Success = false, Message = "Customer not found" });
            return Ok(new GeneralGetResponse { Message = "Customer updated" });
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _customerService.DeleteAsync(id);
            if (!ok) return NotFound(new GeneralGetResponse { Success = false, Message = "Customer not found" });
            return Ok(new GeneralGetResponse { Message = "Customer deleted" });
        }
    }
}
