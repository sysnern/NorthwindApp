using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Concrete
{
    public class ProductService : GenericService<Product, ProductDTO, ProductCreateDto, ProductUpdateDto, int>, IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(
            IProductRepository productRepo,
            IMapper mapper,
            ICacheService cacheService)
            : base(productRepo, mapper, cacheService, "product_list_", "Ürün")
        {
            _productRepo = productRepo;
        }

        // Override to handle ProductFilterDto specifically
        public async Task<ApiResponse<List<ProductDTO>>> GetAllAsync(ProductFilterDto filter)
        {
            // Build filter expression
            Expression<Func<Product, bool>>? filterExpression = null;
            
            if (!IsEmptyFilter(filter))
            {
                filterExpression = p =>
                (string.IsNullOrEmpty(filter.ProductName) || p.ProductName.Contains(filter.ProductName)) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId) &&
                (!filter.MinPrice.HasValue || p.UnitPrice >= filter.MinPrice) &&
                (!filter.MaxPrice.HasValue || p.UnitPrice <= filter.MaxPrice) &&
                (!filter.Discontinued.HasValue || p.Discontinued == filter.Discontinued);
            }

            return await base.GetAllAsync(filterExpression);
        }

        protected override int GetIdFromUpdateDto(ProductUpdateDto dto)
        {
            return dto.ProductId;
        }

        protected override bool SupportsSoftDelete() => true;
        
        protected override void PerformSoftDelete(Product entity)
        {
            entity.Discontinued = true;
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
        
        private static bool IsEmptyFilter(ProductFilterDto filter)
        {
            return string.IsNullOrEmpty(filter.ProductName) &&
                   !filter.CategoryId.HasValue &&
                   !filter.MinPrice.HasValue &&
                   !filter.MaxPrice.HasValue &&
                   !filter.Discontinued.HasValue;
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
