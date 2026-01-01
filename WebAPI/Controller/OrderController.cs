using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var orderId = await _orderService.CreateOrderAsync(request);
            return Ok(new { OrderId = orderId, Message = "Order created successfully" });
        }

        // Read all (staff)
        [HttpGet]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAll()
        {
            var list = await _orderService.GetAllAsync();
            return Ok(new GeneralGetResponse { Data = list });
        }

        // Read by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound(new GeneralBoolResponse { Success = false, Message = "Order not found" });
            return Ok(new GeneralGetResponse { Data = order });
        }

        // Update
        [HttpPut("{id}")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateOrderRequest request)
        {
            request.OrderId = id;
            var ok = await _orderService.UpdateOrderAsync(request);
            if (!ok) return NotFound(new GeneralBoolResponse { Success = false, Message = "Order not found or not updatable" });
            return Ok(new GeneralBoolResponse { Success = true, Message = "Order updated" });
        }

        // Delete (soft)
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _orderService.DeleteOrderAsync(id);
            if (!ok) return NotFound(new GeneralBoolResponse { Success = false, Message = "Order not found" });
            return Ok(new GeneralBoolResponse { Success = true, Message = "Order deleted" });
        }

        [HttpPost("confirm")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> Confirm([FromBody] IdRequest request)
        {
            await _orderService.ConfirmOrderAsync(request.Id);
            return Ok(new GeneralBoolResponse { Message = "Order confirmed" });
        }

        [HttpPost("ship")]
        //[Authorize(Roles = "Staff")]
        public async Task<IActionResult> Ship([FromBody] IdRequest request)
        {
            await _orderService.ShipOrderAsync(request.Id);
            return Ok(new GeneralBoolResponse { Message = "Order marked delivered" });
        }

        [HttpPost("cancel")]
        // [Authorize(Roles = "Staff")]
        public async Task<IActionResult> Cancel([FromBody] IdRequest request)
        {
            await _orderService.CancelOrderAsync(request.Id);
            return Ok(new GeneralBoolResponse { Message = "Order cancelled" });
        }
    }
}
