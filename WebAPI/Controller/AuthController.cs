using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

//POST: TẠO MỚI ĐỐI TƯỢNG HOẶC CẬP NHẬT
//PUT: CHỈ CẬP NHẬT ĐỐI TƯỢNG ĐÃ TỒN TẠI, KHI CẬP NHẬT THÌ CẬP NHẬT TOÀN BỘ THÔNG TIN
//PATCH: CHỈ CẬP NHẬT NHỮNG THÔNG TIN CẦN THIẾT
//DELETE: XÓA ĐỐI TƯỢNG
//GET: LẤY THÔNG TIN ĐỐI TƯỢNG

//CẤU TRÚC HTTP REQUEST
//REQUEST LINE: METHOD + URL + VERSION
//REQUEST HEADER: THÔNG TIN VỀ REQUEST: CONTENT TYPE, AUTHORIZATION, USER AGENT, TOKEN
//REQUEST BODY: DỮ LIỆU GỬI LÊN SERVER (JSON, XML, FORM DATA)

//CẤU TRÚC HTTP RESPONSE
//RESPONSE LINE: VERSION + STATUS CODE + STATUS MESSAGE
//RESPONSE HEADER: THÔNG TIN VỀ RESPONSE: CONTENT TYPE, AUTHORIZATION, SERVER
//RESPONSE BODY: DỮ LIỆU TRẢ VỀ CHO CLIENT (JSON, XML, HTML)

//{
//  phone: "0123456789",
//  password: "password1"
//}

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                var response = new GeneralGetResponse
                {
                    Message = "User logged in successfully",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new GeneralBoolResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return Conflict(response);
            }

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(new { success = result });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                var result = await
                    _authService.RefreshTokenAsync(refreshTokenRequest.RefreshToken);
                var response = new GeneralGetResponse
                {
                    Message = "Làm mới token thành công",
                    Data = result
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new GeneralBoolResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return Conflict(response);
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest logoutRequest)
        {
            try
            {
                var result = await _authService.LogoutAsync(logoutRequest.RefreshToken);
                var response = new GeneralBoolResponse
                {
                    Success = result,
                    Message = "Đăng xuất thành công"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new GeneralBoolResponse
                {
                    Success = false,
                    Message = ex.Message
                };
                return Conflict(response);
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Lấy AccountId từ Claims 
            var accountId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(accountId))
            {
                return Unauthorized(
                    new GeneralGetResponse
                    {
                        Success = false,
                        Message = "Không xác định được người dùng."
                    });
            }
            // Gán accountId vào request
            request.AccountId = accountId;

            try
            {
                var ok = await _authService.ChangePasswordAsync(request);
                if (ok)
                {
                    return Ok(
                        new GeneralGetResponse
                        {
                            Success = true,
                            Message = "Đổi mật khẩu thành công. Vui lòng đăng nhập lại"
                        });
                }
                return BadRequest(
                    new GeneralGetResponse
                    {
                        Success = false,
                        Message = "Đổi mật khẩu không thành công"
                    });
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new GeneralGetResponse
                    {
                        Success = false,
                        Message = ex.Message
                    });
            }
        }
    }
}
