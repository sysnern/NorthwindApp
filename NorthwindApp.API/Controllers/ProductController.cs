using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services;
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
            return Ok(ApiResponse<List<ProductDTO>>.SuccessResponse(result, "Ürünler başarıyla listelendi."));
        }

        // Tekil ürün getirme
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result == null)
                return Ok(ApiResponse<ProductDTO>.Fail("Ürün bulunamadı."));

            return Ok(ApiResponse<ProductDTO>.SuccessResponse(result, "Ürün başarıyla getirildi."));
        }

        // Ürün ekleme
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] ProductCreateDto dto)
        {
            await _productService.AddAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Ürün başarıyla eklendi."));
        }

        // Ürün güncelleme
        [HttpPut]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] ProductUpdateDto dto)
        {
            await _productService.UpdateAsync(dto);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Ürün başarıyla güncellendi."));
        }

        // Ürün silme (soft delete)
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            await _productService.DeleteAsync(id);
            return Ok(ApiResponse<string>.SuccessResponse(null, "Ürün pasif hale getirildi (soft delete uygulandı)."));
        }
    }
}
