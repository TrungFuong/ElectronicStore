using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // CREATE FULL PRODUCT (product + variations + specs + images)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
        {
            var result = await _productService.CreateProductAsync(request);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Create product successfully",
                Data = result
            });
        }

        // GET ALL PRODUCTS
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _productService.GetAllAsync();

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get products successfully",
                Data = data
            });
        }

        // GET PRODUCT BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Data = product
            });
        }

        // UPDATE PRODUCT INFO
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductRequest request)
        {
            request.ProductId = id;
            var ok = await _productService.UpdateProductAsync(request);

            if (!ok)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Update product successfully"
            });
        }

        // SOFT DELETE PRODUCT
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var ok = await _productService.DeleteProductAsync(id);

            if (!ok)
            {
                return NotFound(new GeneralBoolResponse
                {
                    Success = false,
                    Message = "Product not found"
                });
            }

            return Ok(new GeneralBoolResponse
            {
                Success = true,
                Message = "Delete product successfully"
            });
        }

        // GET PRODUCTS BY CATEGORY
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(string categoryId)
        {
            var products = await _productService.GetByCategoryAsync(categoryId);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get products by category successfully",
                Data = products
            });
        }

        // GET PRODUCTS BY BRAND
        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetByBrand(string brandId)
        {
            var products = await _productService.GetByBrandAsync(brandId);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get products by brand successfully",
                Data = products
            });
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty");

            var ext = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid() + ext;

            var folder = Path.Combine("wwwroot/images/products");
            Directory.CreateDirectory(folder);

            var path = Path.Combine(folder, fileName);
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            var url = $"{Request.Scheme}://{Request.Host}/images/products/{fileName}";

            return Ok(new { imageUrl = url });
        }
    }
}
