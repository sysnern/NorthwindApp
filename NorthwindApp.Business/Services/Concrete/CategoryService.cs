using AutoMapper;
using Microsoft.Extensions.Logging;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Concrete
{
    public class CategoryService : GenericService<Category, CategoryDTO, CategoryCreateDto, CategoryUpdateDto, int>, ICategoryService
    {
        public CategoryService(
            ICategoryRepository categoryRepo,
            IMapper mapper,
            ICacheService cacheService,
            ILogger<CategoryService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(categoryRepo, mapper, cacheService, logger, httpContextAccessor, "category_list_", "Kategori")
        {
        }

        // Override to handle CategoryFilterDto specifically with pagination and sorting
        public async Task<ApiResponse<List<CategoryDTO>>> GetAllAsync(CategoryFilterDto? filter = null)
        {
            // Build filter expression
            Expression<Func<Category, bool>>? filterExpression = null;
            
            if (filter != null)
            {
                filterExpression = c =>
                (string.IsNullOrEmpty(filter.CategoryName) || c.CategoryName.ToLower().Contains(filter.CategoryName.ToLower())) &&
                (!filter.IsDeleted.HasValue || c.IsDeleted == filter.IsDeleted);
            }

            // Extract pagination and sorting parameters from filter
            var sortField = filter?.SortField;
            var sortDirection = filter?.SortDirection;
            var page = filter?.Page ?? 1;
            var pageSize = filter?.PageSize ?? 10;

            // Generate cache key with filter parameters
            var cacheKey = GenerateCacheKeyWithFilter(filter);

            return await base.GetAllAsync(filterExpression, sortField, sortDirection, page, pageSize, cacheKey);
        }

        private string GenerateCacheKeyWithFilter(CategoryFilterDto? filter)
        {
            if (filter == null)
                return _cachePrefix;

            // Create a unique cache key based on filter parameters
            var filterParams = new List<string>
            {
                $"cn:{filter.CategoryName ?? ""}",
                $"del:{filter.IsDeleted?.ToString() ?? ""}",
                $"sort:{filter.SortField ?? ""}",
                $"dir:{filter.SortDirection ?? ""}",
                $"page:{filter.Page}",
                $"size:{filter.PageSize}"
            };

            var filterString = string.Join("_", filterParams);
            var hash = filterString.GetHashCode();
            return $"{_cachePrefix}_{hash}";
        }

        protected override int GetIdFromUpdateDto(CategoryUpdateDto dto)
        {
            return dto.CategoryId;
        }

        protected override bool SupportsSoftDelete() => true;

        protected override void PerformSoftDelete(Category entity)
        {
            entity.IsDeleted = true;
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
