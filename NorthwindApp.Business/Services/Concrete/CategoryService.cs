using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Concrete
{
    public class CategoryService : GenericService<Category, CategoryDTO, CategoryCreateDto, CategoryUpdateDto, int>, ICategoryService
    {

        public CategoryService(
            ICategoryRepository categoryRepo,
            IMapper mapper,
            ICacheService cacheService)
            : base(categoryRepo, mapper, cacheService, "category_list_", "Kategori")
        {
        }

        protected override int GetIdFromUpdateDto(CategoryUpdateDto dto)
        {
            return dto.CategoryId;
        }

        // Implement ICategoryService specific methods
        public async Task<ApiResponse<List<CategoryDTO>>> GetAllAsync(CategoryFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Category, bool>>? filterExpression = null;
            
            if (filter != null)
            {
                filterExpression = c =>
                    (string.IsNullOrEmpty(filter.CategoryName) || c.CategoryName.Contains(filter.CategoryName)) &&
                    (!filter.IsDeleted.HasValue || c.IsDeleted == filter.IsDeleted);
            }

            return await base.GetAllAsync(filterExpression);
        }

        public override async Task<ApiResponse<CategoryDTO>> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        async Task<ApiResponse<string>> ICategoryService.AddAsync(CategoryCreateDto dto)
        {
            var result = await base.AddAsync(dto);
            if (result.Success)
            {
                return ApiResponse<string>.Created(null, result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        async Task<ApiResponse<string>> ICategoryService.UpdateAsync(CategoryUpdateDto dto)
        {
            var result = await base.UpdateAsync(dto);
            if (result.Success)
            {
                return ApiResponse<string>.Ok(null, result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        public override async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            if (result.Success)
            {
                return ApiResponse<string>.NoContent(result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }
    }
}
