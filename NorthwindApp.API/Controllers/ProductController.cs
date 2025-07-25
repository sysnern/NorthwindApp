using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using Swashbuckle.AspNetCore.Annotations;

namespace NorthwindApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("list")]
        [SwaggerOperation(Summary = "List products with optional filters")]
        public async Task<ActionResult<ApiResponse<List<ProductDTO>>>> GetAll([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get product by id")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Add a new product")]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] ProductCreateDto dto)
        {
            var result = await _productService.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update an existing product")]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] ProductUpdateDto dto)
        {
            var result = await _productService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Soft delete a product")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
