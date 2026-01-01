using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Implementations;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

//CHƯA AUTHORIZE 


[ApiController]
[Route("api/attributes")]
public class AttributeController : ControllerBase
{
    private readonly IAttributeService _attributeService;

    public AttributeController(IAttributeService attributeService)
    {
        _attributeService = attributeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _attributeService.GetAllAsync();

        return Ok(new GeneralGetResponse
        {
            Success = true,
            Data = data
        });
    }
}