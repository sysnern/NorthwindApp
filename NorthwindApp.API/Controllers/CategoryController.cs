using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using System.ComponentModel.DataAnnotations;

namespace NorthwindApp.API.Controllers
{
    /// <summary>
    /// Kategori yönetimi için API endpoint'leri
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "Categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ICacheService _cacheService;

        public CategoryController(ICategoryService categoryService, ICacheService cacheService)
        {
            _categoryService = categoryService;
            _cacheService = cacheService;
        }

        /// <summary>
        /// Tüm kategorileri getirir (filtreleme ve sayfalama ile)
        /// </summary>
        /// <param name="filter">Kategori filtreleme parametreleri</param>
        /// <returns>Filtrelenmiş kategori listesi</returns>
        /// <response code="200">Kategoriler başarıyla getirildi</response>
        /// <response code="400">Geçersiz filtre parametreleri</response>
        /// <response code="404">Kategori bulunamadı</response>
        [HttpGet("list")]
        [ProducesResponseType(typeof(ApiResponse<List<CategoryDTO>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<ActionResult<ApiResponse<List<CategoryDTO>>>> GetAll([FromQuery] CategoryFilterDto? filter = null)
        {
            var result = await _categoryService.GetAllAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// ID'ye göre kategori getirir
        /// </summary>
        /// <param name="id">Kategori ID'si</param>
        /// <returns>Belirtilen ID'ye sahip kategori</returns>
        /// <response code="200">Kategori başarıyla getirildi</response>
        /// <response code="404">Kategori bulunamadı</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CategoryDTO>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<ActionResult<ApiResponse<CategoryDTO>>> GetById([Range(1, int.MaxValue, ErrorMessage = "ID pozitif bir sayı olmalıdır")] int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Yeni kategori oluşturur
        /// </summary>
        /// <param name="dto">Oluşturulacak kategori bilgileri</param>
        /// <returns>Oluşturulan kategori bilgisi</returns>
        /// <response code="200">Kategori başarıyla oluşturuldu</response>
        /// <response code="400">Geçersiz kategori bilgileri</response>
        /// <response code="409">Kategori zaten mevcut</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 409)]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] CategoryCreateDto dto)
        {
            var result = await _categoryService.AddAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Mevcut kategoriyi günceller
        /// </summary>
        /// <param name="dto">Güncellenecek kategori bilgileri</param>
        /// <returns>Güncelleme sonucu</returns>
        /// <response code="200">Kategori başarıyla güncellendi</response>
        /// <response code="400">Geçersiz kategori bilgileri</response>
        /// <response code="404">Güncellenecek kategori bulunamadı</response>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 400)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] CategoryUpdateDto dto)
        {
            var result = await _categoryService.UpdateAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// Kategoriyi siler (soft delete)
        /// </summary>
        /// <param name="id">Silinecek kategori ID'si</param>
        /// <returns>Silme sonucu</returns>
        /// <response code="200">Kategori başarıyla silindi</response>
        /// <response code="404">Silinecek kategori bulunamadı</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 404)]
        public async Task<ActionResult<ApiResponse<string>>> Delete([Range(1, int.MaxValue, ErrorMessage = "ID pozitif bir sayı olmalıdır")] int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Kategori cache'ini temizler
        /// </summary>
        /// <returns>Cache temizleme sonucu</returns>
        /// <response code="200">Cache başarıyla temizlendi</response>
        [HttpPost("clear-cache")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        public ActionResult<ApiResponse<string>> ClearCache()
        {
            _cacheService.RemoveByPrefix("category_list_");
            return Ok(ApiResponse<string>.Ok(null, "Cache temizlendi."));
        }
    }
}
