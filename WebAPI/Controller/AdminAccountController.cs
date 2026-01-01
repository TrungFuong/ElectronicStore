using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/admin/accounts")]
    [Authorize(Roles = "Admin")]
    public class AdminAccountsController : ControllerBase
    {
        private readonly IAdminAccountService _service;

        public AdminAccountsController(IAdminAccountService service)
        {
            _service = service;
        }

        [HttpPost("create-staff")]
        public async Task<IActionResult> CreateStaff(
            [FromBody] AddStaffRequest request)
        {
            try
            {
                var ok = await _service.AddStaffAsync(request);

                return Ok(new GeneralBoolResponse
                {
                    Success = ok,
                    Message = "Tạo tài khoản nhân viên thành công"
                });
            }
            catch (Exception ex)
            {
                return Conflict(new GeneralBoolResponse
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
