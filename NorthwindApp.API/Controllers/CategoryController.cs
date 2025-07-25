using Microsoft.AspNetCore.Mvc;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using Swashbuckle.AspNetCore.Annotations;

namespace NorthwindApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("list")]
        [SwaggerOperation(Summary = "List categories")]
        public async Task<ActionResult<ApiResponse<List<CategoryDTO>>>> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get category by id")]
        public async Task<ActionResult<ApiResponse<CategoryDTO>>> GetById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Add a new category")]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] CategoryCreateDto dto)
        {
            var result = await _categoryService.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        [SwaggerOperation(Summary = "Update category")]
        public async Task<ActionResult<ApiResponse<string>>> Update([FromBody] CategoryUpdateDto dto)
        {
            var result = await _categoryService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete category")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
