using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

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

        // Listeleme (parametreli filtreleme)
        [HttpGet("list")]
        public async Task<ActionResult<ApiResponse<List<ProductDTO>>>> GetAll([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetAllAsync(filter);
            return Ok(result);
        }

        // Tekil ürün getirme
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(result);
        }

        // Ürün ekleme
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] ProductCreateDto dto)
        {
            var result = await _productService.AddAsync(dto);
            return Ok(result);
        }

        // Ürün güncelleme
        [HttpPut]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] ProductUpdateDto dto)
        {
            var result = await _productService.UpdateAsync(dto);
            return Ok(result);
        }

        // Ürün silme (soft delete)
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
