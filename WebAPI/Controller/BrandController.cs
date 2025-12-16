using Application.DTOs.Auth;
using Application.DTOs.Requests;
using Application.Interfaces;
using Domain.Entities;
using Domain.Models;
using Domain.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/brands")]
public class BrandController : ControllerBase
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    // POST api/brands
    [HttpPost]
    public async Task<IActionResult> Create(CreateBrandRequest request)
    {
        var response = new GeneralBoolResponse();

        try
        {
            await _brandService.CreateAsync(request);
            response.Success = true;
            response.Message = "tạo brand thành công";
            return Ok(response);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return BadRequest(response);
        }
    }

    // GET api/brands
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
    public async Task<IActionResult> Update(UpdateBrandRequest request)
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

    [HttpDelete]
    public async Task<IActionResult> Delete(DeleteBrandRequest request)
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
            response.Message = "Xóa thành công";
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