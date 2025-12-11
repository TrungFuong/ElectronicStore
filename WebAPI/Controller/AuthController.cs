using Application.DTOs.Auth;
using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Requests;
using Microsoft.AspNetCore.Mvc;

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
    }
}
