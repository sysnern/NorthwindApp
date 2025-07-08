using AutoMapper;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<CategoryDTO>>> GetAllAsync()
        {
            var categories = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<CategoryDTO>>(categories);

            if (dtoList == null || !dtoList.Any())
                return ApiResponse<List<CategoryDTO>>.Fail("Hiç kategori bulunamadı.");

            return ApiResponse<List<CategoryDTO>>.SuccessResponse(dtoList, "Kategoriler başarıyla listelendi.");
        }

        public async Task<ApiResponse<CategoryDTO>> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
                return ApiResponse<CategoryDTO>.Fail("Kategori bulunamadı.");

            var dto = _mapper.Map<CategoryDTO>(category);
            return ApiResponse<CategoryDTO>.SuccessResponse(dto, "Kategori başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(CategoryCreateDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _repo.AddAsync(category);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Kategori başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(CategoryUpdateDto dto)
        {
            var category = await _repo.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return ApiResponse<string>.Fail("Güncellenecek kategori bulunamadı.");

            _mapper.Map(dto, category);
            _repo.Update(category);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Kategori başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null)
                return ApiResponse<string>.Fail("Silinecek kategori bulunamadı.");

            _repo.Delete(category);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Kategori başarıyla silindi.");
        }
    }
}
