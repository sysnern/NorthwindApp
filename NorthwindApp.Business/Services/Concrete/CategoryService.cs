using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Entities.Models;

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

<<<<<<< HEAD
        public async Task<ApiResponse<List<CategoryDTO>>> GetAllAsync(CategoryFilterDto? filter = null)
        {
            // 1) Filter null ise default değerler ata
            filter ??= new CategoryFilterDto();

            // 2) Cache key oluştur
            var cacheKey = CachePrefix +
                $"{filter.CategoryName}_{filter.IsDeleted}_{filter.SortField}_{filter.SortDirection}_{filter.Page}_{filter.PageSize}";

            // 3) Cache kontrolü - Cache'den total count da al
            var cacheKeyWithTotal = cacheKey + "_total";
            var cached = _cacheService.Get<List<CategoryDTO>>(cacheKey);
            var cachedTotal = _cacheService.Get<int?>(cacheKeyWithTotal);
            
            if (cached != null && cachedTotal.HasValue)
            {
                return ApiResponse<List<CategoryDTO>>
                    .Ok(cached, "Kategoriler cache'den getirildi.", cachedTotal.Value, filter.Page, filter.PageSize);
            }

            // 4) DB'den çek - Önce total count hesapla
            var allCategories = await _repo.GetAllAsync(c => 
                (!filter.IsDeleted.HasValue || c.IsDeleted == filter.IsDeleted) && // Soft delete filter
                (string.IsNullOrEmpty(filter.CategoryName) || c.CategoryName.Contains(filter.CategoryName))
            );

            // 5) Boş sonuçsa 404
            if (!allCategories.Any())
            {
                return ApiResponse<List<CategoryDTO>>
                    .NotFound("Hiç kategori bulunamadı.");
            }

            // 6) Sorting uygula
            var sortedCategories = allCategories.AsQueryable();
            if (!string.IsNullOrEmpty(filter.SortField))
            {
                sortedCategories = filter.SortField.ToLower() switch
                {
                    "categoryid" => filter.SortDirection == "desc" ? sortedCategories.OrderByDescending(c => c.CategoryId) : sortedCategories.OrderBy(c => c.CategoryId),
                    "categoryname" => filter.SortDirection == "desc" ? sortedCategories.OrderByDescending(c => c.CategoryName) : sortedCategories.OrderBy(c => c.CategoryName),
                    "isdeleted" => filter.SortDirection == "desc" ? sortedCategories.OrderByDescending(c => c.IsDeleted) : sortedCategories.OrderBy(c => c.IsDeleted),
                    _ => sortedCategories.OrderBy(c => c.CategoryId)
                };
            }

            // 7) Total count hesapla (sorting'den sonra)
            var totalCount = sortedCategories.Count();

            // 8) Pagination uygula
            var pagedList = sortedCategories
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var dtoList = _mapper.Map<List<CategoryDTO>>(pagedList);

            // 9) Cache'e yaz (paged result ve total count)
            _cacheService.Set(cacheKey, dtoList);
            _cacheService.Set(cacheKeyWithTotal, totalCount);

            // 10) Başarılı listeleme (total count ile birlikte)
            return ApiResponse<List<CategoryDTO>>
                .Ok(dtoList, "Kategoriler başarıyla listelendi.", totalCount, filter.Page, filter.PageSize);
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

        public async Task<ApiResponse<string>> AddAsync(CategoryCreateDto dto)
        {
            var entity = _mapper.Map<Category>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Created("Kategori başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(CategoryUpdateDto dto)
        {
            var category = await _repo.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return ApiResponse<string>
                    .NotFound("Güncellenecek kategori bulunamadı.");
            }

            _mapper.Map(dto, category);
            _repo.Update(category);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Ok("Kategori başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek kategori bulunamadı.");
            }

            // Soft delete - IsDeleted set et
            category.IsDeleted = true;
            _repo.Update(category);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Kategori başarıyla silindi.");
=======
        protected override int GetIdFromUpdateDto(CategoryUpdateDto dto)
        {
            return dto.CategoryId;
>>>>>>> 46be2e785b2a73d21b3c223b730360640f942087
        }
        
        // ICategoryService already matches the generic service interface
        // No additional implementation needed
    }
}
