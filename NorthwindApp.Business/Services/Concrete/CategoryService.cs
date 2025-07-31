using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
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

        protected override int GetIdFromUpdateDto(CategoryUpdateDto dto)
        {
            return dto.CategoryId;
        }
        
        // ICategoryService already matches the generic service interface
        // No additional implementation needed
    }
}
