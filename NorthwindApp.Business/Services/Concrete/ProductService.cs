﻿using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NorthwindApp.Business.Services.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CachePrefix = "product_list_";

        public ProductService(
            IProductRepository repo,
            IMapper mapper,
            ICacheService cacheService)
        {
            _repo = repo;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<List<ProductDTO>>> GetAllAsync(ProductFilterDto filter)
        {
            // Cache key'i filtre parametrelerinden oluştur
            var cacheKey = CachePrefix +
                $"{filter.ProductName}_{filter.CategoryId}_{filter.MinPrice}_{filter.MaxPrice}_{filter.Discontinued}";

            // Cache'de mevcut mu?
            var cached = _cacheService.Get<List<ProductDTO>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<ProductDTO>>.SuccessResponse(cached, "Ürünler cache'den getirildi.");
            }

            // DB'den veriyi çek
            var products = await _repo.GetAllAsync(p =>
                (string.IsNullOrEmpty(filter.ProductName) || p.ProductName.Contains(filter.ProductName)) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId) &&
                (!filter.MinPrice.HasValue || p.UnitPrice >= filter.MinPrice) &&
                (!filter.MaxPrice.HasValue || p.UnitPrice <= filter.MaxPrice) &&
                (!filter.Discontinued.HasValue || p.Discontinued == filter.Discontinued)
            );

            var dtoList = _mapper.Map<List<ProductDTO>>(products);
            if (dtoList == null || dtoList.Count == 0)
            {
                return ApiResponse<List<ProductDTO>>.Fail("Hiç ürün bulunamadı.");
            }

            // Cache'e yaz 
            _cacheService.Set(cacheKey, dtoList);

            return ApiResponse<List<ProductDTO>>.SuccessResponse(dtoList, "Ürünler başarıyla listelendi.");
        }

        public async Task<ApiResponse<ProductDTO>> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product == null)
            {
                return ApiResponse<ProductDTO>.Fail("Ürün bulunamadı.");
            }

            var dto = _mapper.Map<ProductDTO>(product);
            return ApiResponse<ProductDTO>.SuccessResponse(dto, "Ürün başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(ProductCreateDto dto)
        {
            var entity = _mapper.Map<Product>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>.SuccessResponse(null, "Ürün başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(ProductUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(dto.ProductId);
            if (existing == null)
            {
                return ApiResponse<string>.Fail("Güncellenecek ürün bulunamadı.");
            }

            _mapper.Map(dto, existing);
            _repo.Update(existing);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>.SuccessResponse(null, "Ürün başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
            {
                return ApiResponse<string>.Fail("Silinecek ürün bulunamadı.");
            }

            // Soft delete
            existing.Discontinued = true;
            _repo.Update(existing);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>.SuccessResponse(null, "Ürün pasif hale getirildi (soft delete uygulandı).");
        }
    }
}
