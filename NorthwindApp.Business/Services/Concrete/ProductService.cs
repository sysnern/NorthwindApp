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
    public class ProductService : GenericService<Product, ProductDTO, ProductCreateDto, ProductUpdateDto, int>, IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(
            IProductRepository productRepo,
            IMapper mapper,
            ICacheService cacheService,
            ILogger<ProductService> logger,
            IHttpContextAccessor httpContextAccessor)
            : base(productRepo, mapper, cacheService, logger, httpContextAccessor, "product_list_", "Ürün")
        {
            _productRepo = productRepo;
        }

        // Override to handle ProductFilterDto specifically with pagination and sorting
        public async Task<ApiResponse<List<ProductDTO>>> GetAllAsync(ProductFilterDto? filter = null)
        {
            
            // Build filter expression
            Expression<Func<Product, bool>>? filterExpression = null;
            
            if (filter != null)
            {
                filterExpression = p =>
                (string.IsNullOrEmpty(filter.ProductName) || p.ProductName.ToLower().Contains(filter.ProductName.ToLower())) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId) &&
                (!filter.SupplierId.HasValue || p.SupplierId == filter.SupplierId) &&
                (!filter.MinPrice.HasValue || p.UnitPrice >= filter.MinPrice) &&
                (!filter.MaxPrice.HasValue || p.UnitPrice <= filter.MaxPrice) &&
                (!filter.IsDeleted.HasValue || p.IsDeleted == filter.IsDeleted);
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

        protected override int GetIdFromUpdateDto(ProductUpdateDto dto)
        {
            return dto.ProductId;
        }

        protected override bool SupportsSoftDelete() => true;
        
        protected override void PerformSoftDelete(Product entity)
        {
            entity.IsDeleted = true;
        }

        protected override string GenerateCacheKey(Expression<Func<Product, bool>>? filter)
        {
            if (filter == null)
                return _cachePrefix;
                
            // For product-specific caching with filter details
            var filterString = filter.ToString();
            var hash = filterString.GetHashCode();
            return $"{_cachePrefix}_{hash}";
        }

        private string GenerateCacheKeyWithFilter(ProductFilterDto? filter)
        {
            if (filter == null)
                return _cachePrefix;

            // Create a unique cache key based on filter parameters
            var filterParams = new List<string>
            {
                $"pn:{filter.ProductName ?? ""}",
                $"cid:{filter.CategoryId ?? 0}",
                $"sid:{filter.SupplierId ?? 0}",
                $"minp:{filter.MinPrice ?? 0}",
                $"maxp:{filter.MaxPrice ?? 0}",
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
        
        // Implement IProductService methods that return string instead of ProductDTO
        async Task<ApiResponse<string>> IProductService.AddAsync(ProductCreateDto dto)
        {
            var result = await base.AddAsync(dto);
            if (result.Success)
            {
                return ApiResponse<string>.Created(null, result.Message);
            }
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
        }

        async Task<ApiResponse<string>> IProductService.UpdateAsync(ProductUpdateDto dto)
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
