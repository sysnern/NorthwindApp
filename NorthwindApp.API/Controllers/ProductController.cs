using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services;
using NorthwindApp.Core.DTOs;

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
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterDto filter)
        {
            var result = await _productService.GetAllAsync(filter);
            return Ok(result);
        }

        // Tekil ürün getirme
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result == null)
                return NotFound("Ürün bulunamadı.");
            return Ok(result);
        }

        // Ürün ekleme
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductCreateDto dto)
        {
            await _productService.AddAsync(dto);
            return Ok("Ürün başarıyla eklendi.");
        }

        // Ürün güncelleme
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductUpdateDto dto)
        {
            await _productService.UpdateAsync(dto);
            return Ok("Ürün başarıyla güncellendi.");
        }

        // Ürün silme
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok("Ürün pasif hale getirildi (soft delete uygulandı).");

        }
    }
}
