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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllStaffAsync();
            return Ok(new GeneralGetResponse { Data = list });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var staff = await _service.GetByIdAsync(id);
            if (staff == null) return NotFound(new GeneralBoolResponse { Success = false, Message = "Staff not found" });
            return Ok(new GeneralGetResponse { Data = staff });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateStaffRequest request)
        {
            request.StaffId = id;
            try
            {
                var ok = await _service.UpdateStaffAsync(request);
                if (!ok) return NotFound(new GeneralBoolResponse { Success = false, Message = "Staff not found" });
                return Ok(new GeneralBoolResponse { Success = true, Message = "Staff updated" });
            }
            catch (Exception ex)
            {
                return Conflict(new GeneralBoolResponse { Success = false, Message = ex.Message });
            }
        }

        // New endpoint: toggle staff operational status (enable/disable)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> SetStatus(string id, [FromQuery] bool isActive)
        {
            var ok = await _service.SetStaffStatusAsync(id, isActive);
            if (!ok) return NotFound(new GeneralBoolResponse { Success = false, Message = "Staff not found" });
            return Ok(new GeneralBoolResponse { Success = true, Message = isActive ? "Staff enabled" : "Staff disabled" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _service.DeleteStaffAsync(id);
            if (!ok) return NotFound(new GeneralBoolResponse { Success = false, Message = "Staff not found" });
            return Ok(new GeneralBoolResponse { Success = true, Message = "Staff disabled" });
        }
    }
}
