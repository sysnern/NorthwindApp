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

        // Override to handle CategoryFilterDto specifically with pagination and sorting
        public async Task<ApiResponse<List<CategoryDTO>>> GetAllAsync(CategoryFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Category, bool>>? filterExpression = null;
            
            if (filter != null && !IsEmptyFilter(filter))
            {
                filterExpression = c =>
                (string.IsNullOrEmpty(filter.CategoryName) || c.CategoryName.Contains(filter.CategoryName));
            }

            // Extract pagination and sorting parameters from filter
            var sortField = filter?.SortField;
            var sortDirection = filter?.SortDirection;
            var page = filter?.Page ?? 1;
            var pageSize = filter?.PageSize ?? 10;

            return await base.GetAllAsync(filterExpression, sortField, sortDirection, page, pageSize);
        }

        protected override int GetIdFromUpdateDto(CategoryUpdateDto dto)
        {
            return dto.CategoryId;
        }

        protected override bool SupportsSoftDelete() => false;

        private static bool IsEmptyFilter(CategoryFilterDto filter)
        {
            return string.IsNullOrEmpty(filter.CategoryName);
        }

        // Implement ICategoryService methods that return string instead of CategoryDTO
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
    }
}
