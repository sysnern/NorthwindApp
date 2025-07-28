using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindApp.Business.Services.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CachePrefix = "category_list_";

        public CategoryService(
            ICategoryRepository repo,
            IMapper mapper,
            ICacheService cacheService)
        {
            _repo = repo;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<List<CategoryDTO>>> GetAllAsync()
        {
            // 1) Cache key
            var cacheKey = CachePrefix;

            // 2) Cache kontrolü
            var cached = _cacheService.Get<List<CategoryDTO>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<CategoryDTO>>
                    .Ok(cached, "Kategoriler cache'den getirildi.");
            }

            // 3) DB'den çek
            var categories = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<CategoryDTO>>(categories);

            // 4) Boş sonuçsa 404
            if (!dtoList.Any())
            {
                return ApiResponse<List<CategoryDTO>>
                    .NotFound("Hiç kategori bulunamadı.");
            }

            // 5) Cache'e yaz
            _cacheService.Set(cacheKey, dtoList);

            // 6) Başarılı listeleme
            return ApiResponse<List<CategoryDTO>>
                .Ok(dtoList, "Kategoriler başarıyla listelendi.");
        }

        public async Task<ApiResponse<CategoryDTO>> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
            {
                return ApiResponse<CategoryDTO>
                    .NotFound("Kategori bulunamadı.");
            }

            var dto = _mapper.Map<CategoryDTO>(category);
            return ApiResponse<CategoryDTO>
                .Ok(dto, "Kategori başarıyla getirildi.");
        }

        public async Task<ApiResponse<CategoryDTO>> AddAsync(CategoryCreateDto dto)
        {
            var entity = _mapper.Map<Category>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var createdDto = _mapper.Map<CategoryDTO>(entity);
            return ApiResponse<CategoryDTO>
                .Created(createdDto, "Kategori başarıyla eklendi.");
        }

        public async Task<ApiResponse<CategoryDTO>> UpdateAsync(CategoryUpdateDto dto)
        {
            var category = await _repo.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return ApiResponse<CategoryDTO>
                    .NotFound("Güncellenecek kategori bulunamadı.");
            }

            _mapper.Map(dto, category);
            _repo.Update(category);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var updatedDto = _mapper.Map<CategoryDTO>(category);
            return ApiResponse<CategoryDTO>
                .Ok(updatedDto, "Kategori başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek kategori bulunamadı.");
            }

            _repo.Delete(category);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Kategori başarıyla silindi.");
        }
    }
}
