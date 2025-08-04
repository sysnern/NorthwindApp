using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;
<<<<<<< HEAD
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
=======
using System.Linq.Expressions;
>>>>>>> 46be2e785b2a73d21b3c223b730360640f942087

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

<<<<<<< HEAD
        public async Task<ApiResponse<List<ProductDTO>>> GetAllAsync(ProductFilterDto? filter = null)
        {
            // Filter null ise boş filter oluştur
            filter ??= new ProductFilterDto();

            var cacheKey = CachePrefix +
                $"{filter.ProductName}_{filter.CategoryId}_{filter.SupplierId}_{filter.MinPrice}_{filter.MaxPrice}_{filter.IsDeleted}_{filter.SortField}_{filter.SortDirection}_{filter.Page}_{filter.PageSize}";

            // 1) Cache kontrolü - Cache'den total count da al
            var cacheKeyWithTotal = cacheKey + "_total";
            var cached = _cacheService.Get<List<ProductDTO>>(cacheKey);
            var cachedTotal = _cacheService.Get<int?>(cacheKeyWithTotal);
            
            if (cached != null && cachedTotal.HasValue)
            {
                return ApiResponse<List<ProductDTO>>
                    .Ok(cached, "Ürünler cache'den getirildi.", cachedTotal.Value, filter.Page, filter.PageSize);
            }

            // 2) DB'den çek
            var products = await _repo.GetAllAsync(p =>
                (!filter.IsDeleted.HasValue || p.IsDeleted == filter.IsDeleted) && // Soft delete filter
=======
        // Override to handle ProductFilterDto specifically
        public async Task<ApiResponse<List<ProductDTO>>> GetAllAsync(ProductFilterDto filter)
        {
            // Build filter expression
            Expression<Func<Product, bool>>? filterExpression = null;
            
            if (!IsEmptyFilter(filter))
            {
                filterExpression = p =>
>>>>>>> 46be2e785b2a73d21b3c223b730360640f942087
                (string.IsNullOrEmpty(filter.ProductName) || p.ProductName.Contains(filter.ProductName)) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId) &&
                (!filter.SupplierId.HasValue || p.SupplierId == filter.SupplierId) &&
                (!filter.MinPrice.HasValue || p.UnitPrice >= filter.MinPrice) &&
<<<<<<< HEAD
                (!filter.MaxPrice.HasValue || p.UnitPrice <= filter.MaxPrice)
            );

            var dtoList = _mapper.Map<List<ProductDTO>>(products);

            // 3) Fiyat filtrelerini güvenli hale getir
            if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue)
            {
                // Min ve Max fiyat arasında mantıklı bir aralık var mı kontrol et
                if (filter.MinPrice > filter.MaxPrice)
                {
                    // Geçersiz aralık - boş liste döndür
                    return ApiResponse<List<ProductDTO>>
                        .Ok(new List<ProductDTO>(), "Minimum fiyat maksimum fiyattan büyük olamaz.", 0, filter.Page, filter.PageSize);
                }
            }

            // 4) Varsayılan sıralama: ID'ye göre artan
            if (string.IsNullOrEmpty(filter.SortField))
            {
                dtoList = dtoList.OrderBy(p => p.ProductId).ToList();
            }
            else
            {
                // Custom sorting uygula
                dtoList = filter.SortField.ToLower() switch
                {
                    "productid" => filter.SortDirection?.ToLower() == "desc" 
                        ? dtoList.OrderByDescending(p => p.ProductId).ToList()
                        : dtoList.OrderBy(p => p.ProductId).ToList(),
                    "productname" => filter.SortDirection?.ToLower() == "desc" 
                        ? dtoList.OrderByDescending(p => p.ProductName).ToList()
                        : dtoList.OrderBy(p => p.ProductName).ToList(),
                    "unitprice" => filter.SortDirection?.ToLower() == "desc" 
                        ? dtoList.OrderByDescending(p => p.UnitPrice).ToList()
                        : dtoList.OrderBy(p => p.UnitPrice).ToList(),
                    "isdeleted" => filter.SortDirection?.ToLower() == "desc" 
                        ? dtoList.OrderByDescending(p => p.IsDeleted).ToList()
                        : dtoList.OrderBy(p => p.IsDeleted).ToList(),
                    _ => dtoList.OrderBy(p => p.ProductId).ToList() // Varsayılan ID sıralaması
                };
            }

            // 5) Toplam sayıyı hesapla (pagination öncesi)
            var totalCount = dtoList.Count;

            // 6) Pagination uygula
            var pagedList = dtoList
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            // 7) Herhangi bir sonuç yoksa 404
            if (pagedList.Count == 0)
            {
                return ApiResponse<List<ProductDTO>>
                    .NotFound("Belirtilen kriterlere uygun ürün bulunamadı.");
            }

            // 8) Cache'e yaz (paged result ve total count)
            _cacheService.Set(cacheKey, pagedList);
            _cacheService.Set(cacheKeyWithTotal, totalCount);

            // 9) Başarılı listeleme (total count ile birlikte)
            return ApiResponse<List<ProductDTO>>
                .Ok(pagedList, "Ürünler başarıyla listelendi.", totalCount, filter.Page, filter.PageSize);
=======
                (!filter.MaxPrice.HasValue || p.UnitPrice <= filter.MaxPrice) &&
                (!filter.Discontinued.HasValue || p.Discontinued == filter.Discontinued);
            }

            return await base.GetAllAsync(filterExpression);
>>>>>>> 46be2e785b2a73d21b3c223b730360640f942087
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
<<<<<<< HEAD

            _mapper.Map(dto, existing);
            _repo.Update(existing);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Ok(null, "Ürün başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek ürün bulunamadı.");
            }

            // Soft delete - IsDeleted set et
            existing.IsDeleted = true;
            _repo.Update(existing);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Ürün başarıyla silindi.");
=======
            return ApiResponse<string>.BadRequest(result.Errors ?? new[] { result.Message }, result.Message);
>>>>>>> 46be2e785b2a73d21b3c223b730360640f942087
        }
    }
}
