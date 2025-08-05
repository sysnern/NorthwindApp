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
            return await GetAllAsync(filter, null, null, 1, 10);
        }

        public virtual async Task<ApiResponse<List<TDto>>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            string? sortField = null,
            string? sortDirection = null,
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                // Generate cache key based on filter and pagination
                var cacheKey = GenerateCacheKey(filter, sortField, sortDirection, page, pageSize);

                // Check cache first
                var cached = _cacheService.Get<List<TDto>>(cacheKey);
                if (cached != null)
                {
                    return ApiResponse<List<TDto>>.Ok(cached, $"{_entityName} listesi cache'den getirildi.");
                }

                // Get total count for pagination
                var totalCount = await _repository.CountAsync(filter);

                // Fetch from database with pagination and sorting
                var entities = await _repository.GetAllAsync(filter, sortField, sortDirection, page, pageSize);
                var dtoList = _mapper.Map<List<TDto>>(entities);

                // Handle empty results
                if (!dtoList.Any())
                {
                    return ApiResponse<List<TDto>>.NotFound($"Hiç {_entityName.ToLower()} bulunamadı.");
                }

                // Cache the results
                _cacheService.Set(cacheKey, dtoList, TimeSpan.FromMinutes(5));

                // Add pagination info to response
                var response = ApiResponse<List<TDto>>.Ok(dtoList, $"{_entityName} listesi başarıyla getirildi.");
                response.TotalCount = totalCount;
                response.Page = page;
                response.PageSize = pageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                return response;
            }
            catch (Exception ex)
            {
                return ApiResponse<List<TDto>>.Error($"Veri getirme sırasında hata oluştu: {ex.Message}");
            }
        }

        public virtual async Task<ApiResponse<TDto>> GetByIdAsync(TKey id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return ApiResponse<TDto>.NotFound($"{_entityName} bulunamadı.");
                }

                var dto = _mapper.Map<TDto>(entity);
                return ApiResponse<TDto>.Ok(dto, $"{_entityName} başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ApiResponse<TDto>.Error($"Veri getirme sırasında hata oluştu: {ex.Message}");
            }
        }

        public virtual async Task<ApiResponse<TDto>> AddAsync(TCreateDto dto)
        {
            try
            {
                // Validate business rules before adding
                var validationResult = await ValidateBusinessRulesForCreate(dto);
                if (!validationResult.IsValid)
                {
                    return ApiResponse<TDto>.BadRequest(validationResult.Errors, validationResult.ErrorMessage);
                }

                var entity = _mapper.Map<TEntity>(dto);
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                var resultDto = _mapper.Map<TDto>(entity);
                InvalidateCache();

                return ApiResponse<TDto>.Created(resultDto, $"{_entityName} başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                return ApiResponse<TDto>.Error($"Ekleme sırasında hata oluştu: {ex.Message}");
            }
        }

        public virtual async Task<ApiResponse<TDto>> UpdateAsync(TUpdateDto dto)
        {
            try
            {
                var id = GetIdFromUpdateDto(dto);
                var existingEntity = await _repository.GetByIdAsync(id);

                if (existingEntity == null)
                {
                    return ApiResponse<TDto>.NotFound($"{_entityName} bulunamadı.");
                }

                // Validate business rules before updating
                var validationResult = await ValidateBusinessRulesForUpdate(dto, existingEntity);
                if (!validationResult.IsValid)
                {
                    return ApiResponse<TDto>.BadRequest(validationResult.Errors, validationResult.ErrorMessage);
                }

                _mapper.Map(dto, existingEntity);
                _repository.Update(existingEntity);
                await _repository.SaveChangesAsync();

                var resultDto = _mapper.Map<TDto>(existingEntity);
                InvalidateCache();

                return ApiResponse<TDto>.Ok(resultDto, $"{_entityName} başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return ApiResponse<TDto>.Error($"Güncelleme sırasında hata oluştu: {ex.Message}");
            }
        }

        public virtual async Task<ApiResponse<string>> DeleteAsync(TKey id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    return ApiResponse<string>.NotFound($"{_entityName} bulunamadı.");
                }

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
                InvalidateCache();

                return ApiResponse<string>.Ok(null, $"{_entityName} başarıyla silindi.");
            }
            catch (Exception ex)
            {
                return ApiResponse<string>.Error($"Silme sırasında hata oluştu: {ex.Message}");
            }
        }

        protected virtual string GenerateCacheKey(Expression<Func<TEntity, bool>>? filter)
        {
            if (filter == null)
                return _cachePrefix;
                
            var filterString = filter.ToString();
            var hash = filterString.GetHashCode();
            return $"{_cachePrefix}_{hash}";
        }

        protected virtual string GenerateCacheKey(
            Expression<Func<TEntity, bool>>? filter,
            string? sortField,
            string? sortDirection,
            int page,
            int pageSize)
        {
            var baseKey = GenerateCacheKey(filter);
            var paginationKey = $"p{page}_s{pageSize}";
            var sortKey = string.IsNullOrEmpty(sortField) ? "" : $"sort_{sortField}_{sortDirection}";
            
            return $"{baseKey}_{paginationKey}_{sortKey}";
        }

        protected virtual void InvalidateCache()
        {
            _cacheService.Remove(_cachePrefix);
        }

        protected abstract TKey GetIdFromUpdateDto(TUpdateDto dto);

        protected virtual bool SupportsSoftDelete() => false;

        protected virtual void PerformSoftDelete(TEntity entity) { }

        protected virtual async Task<BusinessValidationResult> ValidateBusinessRulesForCreate(TCreateDto dto)
        {
            // Override in derived classes for specific business rules
            return BusinessValidationResult.Success();
        }

        protected virtual async Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(TUpdateDto dto, TEntity entity)
        {
            // Override in derived classes for specific business rules
            return BusinessValidationResult.Success();
        }
    }

    public class BusinessValidationResult
    {
        public bool IsValid { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
        public string ErrorMessage { get; set; } = string.Empty;

        public static BusinessValidationResult Success()
        {
            return new BusinessValidationResult { IsValid = true };
        }

        public static BusinessValidationResult Failure(string errorMessage, params string[] errors)
        {
            return new BusinessValidationResult 
            { 
                IsValid = false, 
                ErrorMessage = errorMessage, 
                Errors = errors 
            };
        }
    }
}