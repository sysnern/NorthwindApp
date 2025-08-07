using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using System.ComponentModel.DataAnnotations;

namespace NorthwindApp.API.Controllers
{
    /// <summary>
    /// Ürün yönetimi için API endpoint'leri
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICacheService _cacheService;

        public ProductController(IProductService productService, ICacheService cacheService)
        {
            _productService = productService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Tüm ürünleri getirir (filtreleme ve sayfalama ile)
        /// </summary>
        /// <param name="filter">Ürün filtreleme parametreleri</param>
        /// <returns>Filtrelenmiş ürün listesi</returns>
        /// <response code="200">Ürünler başarıyla getirildi</response>
        /// <response code="400">Geçersiz filtre parametreleri</response>
        /// <response code="404">Ürün bulunamadı</response>
        [HttpGet("list")]
        [ProducesResponseType(typeof(ApiResponse<List<ProductDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<ActionResult<ApiResponse<List<ProductDTO>>>> GetAll([FromQuery] ProductFilterDto? filter = null)
        {
            var result = await _productService.GetAllAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// ID'ye göre ürün getirir
        /// </summary>
        /// <param name="id">Ürün ID'si</param>
        /// <returns>Belirtilen ID'ye sahip ürün</returns>
        /// <response code="200">Ürün başarıyla getirildi</response>
        /// <response code="404">Ürün bulunamadı</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> GetById([Range(1, int.MaxValue, ErrorMessage = "ID pozitif bir sayı olmalıdır")] int id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Yeni ürün oluşturur
        /// </summary>
        /// <param name="dto">Oluşturulacak ürün bilgileri</param>
        /// <returns>Oluşturulan ürün bilgisi</returns>
        /// <response code="200">Ürün başarıyla oluşturuldu</response>
        /// <response code="400">Geçersiz ürün bilgileri</response>
        /// <response code="409">Ürün zaten mevcut</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 409)]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] ProductCreateDto dto)
        {
            var result = await _productService.AddAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Mevcut ürünü günceller
        /// </summary>
        /// <param name="dto">Güncellenecek ürün bilgileri</param>
        /// <returns>Güncelleme sonucu</returns>
        /// <response code="200">Ürün başarıyla güncellendi</response>
        /// <response code="400">Geçersiz ürün bilgileri</response>
        /// <response code="404">Güncellenecek ürün bulunamadı</response>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] ProductUpdateDto dto)
        {
            var result = await _productService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Ürünü siler (soft delete)
        /// </summary>
        /// <param name="id">Silinecek ürün ID'si</param>
        /// <returns>Silme sonucu</returns>
        /// <response code="200">Ürün başarıyla silindi</response>
        /// <response code="404">Silinecek ürün bulunamadı</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<ActionResult<ApiResponse<string>>> Delete([Range(1, int.MaxValue, ErrorMessage = "ID pozitif bir sayı olmalıdır")] int id)
        {
            var result = await _productService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Ürün cache'ini temizler
        /// </summary>
        /// <returns>Cache temizleme sonucu</returns>
        /// <response code="200">Cache başarıyla temizlendi</response>
        [HttpPost("clear-cache")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public ActionResult<ApiResponse<string>> ClearCache()
        {
            // Clear all cache for products
            _cacheService.RemoveByPrefix("product_list_");
            return Ok(ApiResponse<string>.Ok(null, "Cache temizlendi."));
        }
    }
}
