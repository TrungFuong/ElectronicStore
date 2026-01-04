using Application.Interfaces;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/revenue")]
    [Authorize(Roles = "Staff")]
    public class RevenueController : ControllerBase
    {
        private readonly IRevenueService _revenueService;

        public RevenueController(IRevenueService revenueService)
        {
            _revenueService = revenueService;
        }

       [HttpGet("day")]
        public async Task<IActionResult> GetByDay([FromQuery] DateOnly day)
        {
            var total = await _revenueService.GetTotalRevenueByDayAsync(day);
            return Ok(new GeneralGetResponse { Data = new { Total = total } });
        }

        [HttpGet("month")]
        
        public async Task<IActionResult> GetByMonth([FromQuery] int year, [FromQuery] int month)
        {
            var total = await _revenueService.GetTotalRevenueByMonthAsync(year, month);
            return Ok(new GeneralGetResponse { Data = new { Total = total } });
        }
    }   
}
