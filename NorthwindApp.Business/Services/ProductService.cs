using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using NorthwindApp.Business.Services;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ProductService(IProductRepository repo, IMapper mapper, IMemoryCache cache)
        {
            _repo = repo;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ApiResponse<List<ProductDTO>>> GetAllAsync(ProductFilterDto filter)
        {
            // Benzersiz cache key oluştur
            var cacheKey = $"product_list_{filter.ProductName}_{filter.CategoryId}_{filter.MinPrice}_{filter.MaxPrice}_{filter.Discontinued}";

            // 1. Cache'de varsa onu döndür
            if (_cache.TryGetValue(cacheKey, out List<ProductDTO>? cachedList))
            {
                return ApiResponse<List<ProductDTO>>.SuccessResponse(cachedList, "Ürünler cache'den getirildi.");
            }

            // 2. Yoksa DB'den çek
            var products = await _repo.GetAllAsync(p =>
                (string.IsNullOrEmpty(filter.ProductName) || p.ProductName.Contains(filter.ProductName)) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId) &&
                (!filter.MinPrice.HasValue || p.UnitPrice >= filter.MinPrice) &&
                (!filter.MaxPrice.HasValue || p.UnitPrice <= filter.MaxPrice) &&
                (!filter.Discontinued.HasValue || p.Discontinued == filter.Discontinued)
            );

            var dtoList = _mapper.Map<List<ProductDTO>>(products);
            if (!dtoList.Any())
                return ApiResponse<List<ProductDTO>>.Fail("Hiç ürün bulunamadı.");

            // 3. Cache'e yaz
            _cache.Set(cacheKey, dtoList, TimeSpan.FromMinutes(5));

            return ApiResponse<List<ProductDTO>>.SuccessResponse(dtoList, "Ürünler başarıyla listelendi.");
        }

        public async Task<ApiResponse<ProductDTO>> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                return ApiResponse<ProductDTO>.Fail("Ürün bulunamadı.");

            var dto = _mapper.Map<ProductDTO>(product);
            return ApiResponse<ProductDTO>.SuccessResponse(dto, "Ürün başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();

            // Cache temizlenmeli
            ClearProductListCache();

            return ApiResponse<string>.SuccessResponse(null, "Ürün başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(ProductUpdateDto dto)
        {
            var product = await _repo.GetByIdAsync(dto.ProductId);
            if (product == null)
                return ApiResponse<string>.Fail("Güncellenecek ürün bulunamadı.");

            _mapper.Map(dto, product);
            _repo.Update(product);
            await _repo.SaveChangesAsync();

            ClearProductListCache();

            return ApiResponse<string>.SuccessResponse(null, "Ürün başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
                return ApiResponse<string>.Fail("Silinecek ürün bulunamadı.");

            product.Discontinued = true; // Soft delete
            _repo.Update(product);
            await _repo.SaveChangesAsync();

            ClearProductListCache();

            return ApiResponse<string>.SuccessResponse(null, "Ürün pasif hale getirildi (soft delete uygulandı).");
        }

        // 🧹 Liste cache'ini temizleyen yardımcı metot
        private void ClearProductListCache()
        {
            // Eğer prefix ile kayıt yapmış olsaydık, tümünü silerdik
            // Ama burada direkt tüm cache'leri silmek mümkün değil
            // İleride ICacheService abstraction ile geliştirilebilir
        }
    }
}
