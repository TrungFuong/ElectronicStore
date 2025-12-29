using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string GetAccountId()
        {
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountId))
                throw new Exception("Không xác định được người dùng.");
            return accountId;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCart()
        {
            try
            {
                var accountId = GetAccountId();
                var data = await _cartService.GetMyCartAsync(accountId);
                return Ok(new GeneralGetResponse { Success = true, Message = "Lấy giỏ hàng thành công", Data = data });
            }
            catch (Exception ex)
            {
                return Conflict(new GeneralBoolResponse { Success = false, Message = ex.Message });
            }
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest req)
        {
            try
            {
                var accountId = GetAccountId();
                var data = await _cartService.AddItemAsync(accountId, req);
                return Ok(new GeneralGetResponse { Success = true, Message = "Thêm vào giỏ thành công", Data = data });
            }
            catch (Exception ex)
            {
                return Conflict(new GeneralBoolResponse { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("items/{variationId}")]
        public async Task<IActionResult> UpdateItem(string variationId, [FromBody] UpdateCartItemRequest req)
        {
            try
            {
                var accountId = GetAccountId();
                var data = await _cartService.UpdateItemAsync(accountId, variationId, req.Quantity);
                return Ok(new GeneralGetResponse { Success = true, Message = "Cập nhật giỏ thành công", Data = data });
            }
            catch (Exception ex)
            {
                return Conflict(new GeneralBoolResponse { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("items/{variationId}")]
        public async Task<IActionResult> RemoveItem(string variationId)
        {
            try
            {
                var accountId = GetAccountId();
                var data = await _cartService.RemoveItemAsync(accountId, variationId);
                return Ok(new GeneralGetResponse { Success = true, Message = "Xóa item thành công", Data = data });
            }
            catch (Exception ex)
            {
                return Conflict(new GeneralBoolResponse { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var accountId = GetAccountId();
                var ok = await _cartService.ClearAsync(accountId);
                return Ok(new GeneralBoolResponse { Success = ok, Message = "Xóa giỏ hàng thành công" });
            }
            catch (Exception ex)
            {
                return Conflict(new GeneralBoolResponse { Success = false, Message = ex.Message });
            }
        }
    }
}
