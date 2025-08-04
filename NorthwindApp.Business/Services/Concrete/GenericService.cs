using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Concrete
{
    /// <summary>
    /// Generic service base class that implements common CRUD operations
    /// Eliminates 90% of duplicate code across all service implementations
    /// </summary>
    public abstract class GenericService<TEntity, TDto, TCreateDto, TUpdateDto, TKey> 
        : IGenericService<TEntity, TDto, TCreateDto, TUpdateDto, TKey>
        where TEntity : class
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        protected readonly IGenericRepository<TEntity, TKey> _repository;
        protected readonly IMapper _mapper;
        protected readonly ICacheService _cacheService;
        protected readonly string _cachePrefix;
        protected readonly string _entityName;

        protected GenericService(
            IGenericRepository<TEntity, TKey> repository,
            IMapper mapper,
            ICacheService cacheService,
            string cachePrefix,
            string entityName)
        {
            _repository = repository;
            _mapper = mapper;
            _cacheService = cacheService;
            _cachePrefix = cachePrefix;
            _entityName = entityName;
        }

        public virtual async Task<ApiResponse<List<TDto>>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            // Generate cache key based on filter
            var cacheKey = GenerateCacheKey(filter);

            // Check cache first
            var cached = _cacheService.Get<List<TDto>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<TDto>>.Ok(cached, $"{_entityName} listesi cache'den getirildi.");
            }

            // Fetch from database
            var entities = await _repository.GetAllAsync(filter);
            var dtoList = _mapper.Map<List<TDto>>(entities);

            // Handle empty results
            if (!dtoList.Any())
            {
                return ApiResponse<List<TDto>>.NotFound($"Hiç {_entityName.ToLower()} bulunamadı.");
            }

            // Cache the results
            _cacheService.Set(cacheKey, dtoList, TimeSpan.FromMinutes(5));

            return ApiResponse<List<TDto>>.Ok(dtoList, $"{_entityName} listesi başarıyla getirildi.");
        }

        public virtual async Task<ApiResponse<TDto>> GetByIdAsync(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return ApiResponse<TDto>.NotFound($"{_entityName} bulunamadı.");
            }

            var dto = _mapper.Map<TDto>(entity);
            return ApiResponse<TDto>.Ok(dto, $"{_entityName} başarıyla getirildi.");
        }

        public virtual async Task<ApiResponse<TDto>> AddAsync(TCreateDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            // Clear cache
            InvalidateCache();

            var createdDto = _mapper.Map<TDto>(entity);
            return ApiResponse<TDto>.Created(createdDto, $"{_entityName} başarıyla eklendi.");
        }

        public virtual async Task<ApiResponse<TDto>> UpdateAsync(TUpdateDto dto)
        {
            var id = GetIdFromUpdateDto(dto);
            var entity = await _repository.GetByIdAsync(id);
            
            if (entity == null)
            {
                return ApiResponse<TDto>.NotFound($"Güncellenecek {_entityName.ToLower()} bulunamadı.");
            }

            _mapper.Map(dto, entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            // Clear cache
            InvalidateCache();

            var updatedDto = _mapper.Map<TDto>(entity);
            return ApiResponse<TDto>.Ok(updatedDto, $"{_entityName} başarıyla güncellendi.");
        }

        public virtual async Task<ApiResponse<string>> DeleteAsync(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return ApiResponse<string>.NotFound($"Silinecek {_entityName.ToLower()} bulunamadı.");
            }

            // Check if soft delete is supported
            if (SupportsSoftDelete())
            {
                PerformSoftDelete(entity);
                _repository.Update(entity);
            }
            else
            {
                _repository.Delete(entity);
            }

            await _repository.SaveChangesAsync();

            // Clear cache
            InvalidateCache();

            var message = SupportsSoftDelete() 
                ? $"{_entityName} pasif hale getirildi (soft delete)."
                : $"{_entityName} başarıyla silindi.";

            return ApiResponse<string>.NoContent(message);
        }

        #region Protected Helper Methods

        protected virtual string GenerateCacheKey(Expression<Func<TEntity, bool>>? filter)
        {
            if (filter == null)
                return _cachePrefix;

            // Simple hash of filter expression for cache key
            var filterString = filter.ToString();
            var hash = filterString.GetHashCode();
            return $"{_cachePrefix}_{hash}";
        }

        protected virtual void InvalidateCache()
        {
            _cacheService.RemoveByPrefix(_cachePrefix);
        }

        protected abstract TKey GetIdFromUpdateDto(TUpdateDto dto);
        
        protected virtual bool SupportsSoftDelete() => false;
        
        protected virtual void PerformSoftDelete(TEntity entity) { }

        #endregion
    }
}