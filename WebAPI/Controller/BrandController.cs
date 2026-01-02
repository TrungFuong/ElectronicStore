using Application.DTOs.Requests;
using Application.Interfaces;
using Application.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/brands")]
public class BrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request)
    {
        var response = new GeneralBoolResponse();

        try
        {
            await _brandService.CreateAsync(request);
            response.Success = true;
            response.Message = "Tạo brand thành công";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = new GeneralGetResponse();

        try
        {
            var brands = await _brandService.GetAllAsync();
            response.Success = true;
            response.Data = brands;
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateBrandRequest request)
    {
        var response = new GeneralBoolResponse();

        try
        {
            var success = await _brandService.UpdateAsync(request);
            if (!success)
            {
                response.Success = false;
                response.Message = "Không tìm thấy brand";
                return NotFound(response);
            }

            response.Success = true;
            response.Message = "Sửa thành công";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }
    }

    [HttpPatch("status")]
    public async Task<IActionResult> SetStatus([FromBody] ToggleBrandStatusRequest request)
    {
        var ok = await _brandService.SetActiveAsync(request.BrandId, request.IsActive);

        if (!ok)
            return NotFound(new GeneralBoolResponse { Success = false, Message = "Không tìm thấy brand" });

        return Ok(new GeneralBoolResponse { Success = true, Message = "Cập nhật trạng thái thành công" });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteBrandRequest request)
    {
        var response = new GeneralBoolResponse();

        try
        {
            var success = await _brandService.DeleteAsync(request);
            if (!success)
            {
                response.Success = false;
                response.Message = "Không tìm thấy brand";
                return NotFound(response);
            }

            response.Success = true;
            response.Message = "Tắt brand (soft delete) thành công";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }
    }
}
