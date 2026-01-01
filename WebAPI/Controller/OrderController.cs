using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Models.Requests;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var orderId = await _orderService.CreateOrderAsync(request);

            return Ok(new
            {
                OrderId = orderId,
                Message = "Order created successfully"
            });
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
