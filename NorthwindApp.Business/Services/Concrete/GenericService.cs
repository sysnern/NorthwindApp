using AutoMapper;
using Microsoft.Extensions.Logging;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

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
        protected readonly ILogger<GenericService<TEntity, TDto, TCreateDto, TUpdateDto, TKey>> _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly string _cachePrefix;
        protected readonly string _entityName;

        protected GenericService(
            IGenericRepository<TEntity, TKey> repository,
            IMapper mapper,
            ICacheService cacheService,
            ILogger<GenericService<TEntity, TDto, TCreateDto, TUpdateDto, TKey>> logger,
            IHttpContextAccessor httpContextAccessor,
            string cachePrefix,
            string entityName)
        {
            _repository = repository;
            _mapper = mapper;
            _cacheService = cacheService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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
            // Generate cache key
            var cacheKey = GenerateCacheKey(filter, sortField, sortDirection, page, pageSize);
            return await GetAllAsync(filter, sortField, sortDirection, page, pageSize, cacheKey);
        }

        public virtual async Task<ApiResponse<List<TDto>>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            string? sortField = null,
            string? sortDirection = null,
            int page = 1,
            int pageSize = 10,
            string? customCacheKey = null)
        {
            try
            {
                // Use custom cache key if provided, otherwise generate one
                var cacheKey = customCacheKey ?? GenerateCacheKey(filter, sortField, sortDirection, page, pageSize);
                
                // Try to get from cache first
                var cachedResult = _cacheService.Get<ApiResponse<List<TDto>>>(cacheKey);
                if (cachedResult != null)
                {
                    // Cache hit - Add to HTTP context for enhanced logging
                    AddBusinessContextToHttpContext("HIT", cachedResult.Data?.Count ?? 0, page, (int)Math.Ceiling((double)(cachedResult.TotalCount ?? 0) / pageSize));
                    return cachedResult;
                }

                // Cache miss - Add to HTTP context for enhanced logging
                AddBusinessContextToHttpContext("MISS", 0, page, 0);

                // Get total count for pagination
                var totalCount = await _repository.CountAsync(filter);

                // Fetch from database with pagination and sorting
                var entities = await _repository.GetAllAsync(filter, sortField, sortDirection, page, pageSize);
                var dtoList = _mapper.Map<List<TDto>>(entities);

                // Handle empty results
                if (!dtoList.Any())
                {
                    var notFoundResponse = ApiResponse<List<TDto>>.NotFound($"Hiç {_entityName.ToLower()} bulunamadı.");
                    AddBusinessContextToHttpContext("EMPTY", 0, page, 0);
                    return notFoundResponse;
                }

                // Add pagination info to response
                var response = ApiResponse<List<TDto>>.Ok(dtoList, $"{_entityName} listesi başarıyla getirildi.");
                response.TotalCount = totalCount;
                response.Page = page;
                response.PageSize = pageSize;
                response.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Cache the result without expiration (until CRUD operation)
                _cacheService.Set(cacheKey, response);

                // Add business context to HTTP context for enhanced logging
                AddBusinessContextToHttpContext("DB", dtoList.Count, page, response.TotalPages ?? 0);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving {EntityName} list", _entityName);
                AddBusinessContextToHttpContext("ERROR", 0, page, 0);
                return ApiResponse<List<TDto>>.Error($"Veri getirme sırasında hata oluştu: {ex.Message}");
            }
        }

        public virtual async Task<ApiResponse<TDto>> GetByIdAsync(TKey id)
        {
            try
            {
                // Generate cache key for single entity
                var cacheKey = $"{_cachePrefix}_id_{id}";
                
                // Try to get from cache first
                var cachedResult = _cacheService.Get<ApiResponse<TDto>>(cacheKey);
                if (cachedResult != null)
                {
                    _logger.LogInformation("Cache hit for {EntityName} with ID: {Id}", _entityName, id);
                    return cachedResult;
                }

                _logger.LogInformation("Cache miss for {EntityName} with ID: {Id}. Fetching from database", _entityName, id);

                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    var notFoundResponse = ApiResponse<TDto>.NotFound($"{_entityName} bulunamadı.");
                    _logger.LogWarning("{EntityName} with ID {Id} not found", _entityName, id);
                    return notFoundResponse;
                }

                var dto = _mapper.Map<TDto>(entity);
                var response = ApiResponse<TDto>.Ok(dto, $"{_entityName} başarıyla getirildi.");
                
                // Cache the result without expiration (until CRUD operation)
                _cacheService.Set(cacheKey, response);

                _logger.LogInformation("{EntityName} with ID {Id} retrieved successfully", _entityName, id);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving {EntityName} with ID: {Id}", _entityName, id);
                return ApiResponse<TDto>.Error($"Veri getirme sırasında hata oluştu: {ex.Message}");
            }
        }

        public virtual async Task<ApiResponse<TDto>> AddAsync(TCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Adding new {EntityName}", _entityName);

                // Validate business rules before adding
                var validationResult = await ValidateBusinessRulesForCreate(dto);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Business validation failed for {EntityName}. Errors: {Errors}", 
                        _entityName, string.Join(", ", validationResult.Errors));
                    return ApiResponse<TDto>.BadRequest(validationResult.Errors, validationResult.ErrorMessage);
                }

                var entity = _mapper.Map<TEntity>(dto);
                await _repository.AddAsync(entity);
                await _repository.SaveChangesAsync();

                var resultDto = _mapper.Map<TDto>(entity);
                InvalidateCache();

                // Get the ID from the entity using reflection since we can't cast TDto to TUpdateDto
                var idProperty = typeof(TEntity).GetProperty("Id") ?? 
                                typeof(TEntity).GetProperty("ProductId") ?? 
                                typeof(TEntity).GetProperty("CategoryId") ?? 
                                typeof(TEntity).GetProperty("SupplierId") ?? 
                                typeof(TEntity).GetProperty("EmployeeId") ?? 
                                typeof(TEntity).GetProperty("OrderId") ?? 
                                typeof(TEntity).GetProperty("CustomerId");
                
                var id = idProperty?.GetValue(entity)?.ToString() ?? "Unknown";

                _logger.LogInformation("{EntityName} added successfully. ID: {Id}", _entityName, id);

                return ApiResponse<TDto>.Created(resultDto, $"{_entityName} başarıyla eklendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding {EntityName}", _entityName);
                return ApiResponse<TDto>.Error($"Ekleme sırasında hata oluştu: {ex.Message}");
            }
        }

        public virtual async Task<ApiResponse<TDto>> UpdateAsync(TUpdateDto dto)
        {
            try
            {
                var id = GetIdFromUpdateDto(dto);
                _logger.LogInformation("Updating {EntityName} with ID: {Id}", _entityName, id);

                var existingEntity = await _repository.GetByIdAsync(id);

                if (existingEntity == null)
                {
                    _logger.LogWarning("{EntityName} with ID {Id} not found for update", _entityName, id);
                    return ApiResponse<TDto>.NotFound($"{_entityName} bulunamadı.");
                }

                // Validate business rules before updating
                var validationResult = await ValidateBusinessRulesForUpdate(dto, existingEntity);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Business validation failed for {EntityName} update. ID: {Id}, Errors: {Errors}", 
                        _entityName, id, string.Join(", ", validationResult.Errors));
                    return ApiResponse<TDto>.BadRequest(validationResult.Errors, validationResult.ErrorMessage);
                }

                _mapper.Map(dto, existingEntity);
                _repository.Update(existingEntity);
                await _repository.SaveChangesAsync();

                var resultDto = _mapper.Map<TDto>(existingEntity);
                InvalidateCache();

                _logger.LogInformation("{EntityName} with ID {Id} updated successfully", _entityName, id);

                return ApiResponse<TDto>.Ok(resultDto, $"{_entityName} başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating {EntityName} with ID: {Id}", _entityName, GetIdFromUpdateDto(dto));
                return ApiResponse<TDto>.Error($"Güncelleme sırasında hata oluştu: {ex.Message}");
            }
        }

        public virtual async Task<ApiResponse<string>> DeleteAsync(TKey id)
        {
            try
            {
                _logger.LogInformation("Deleting {EntityName} with ID: {Id}", _entityName, id);

                var entity = await _repository.GetByIdAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("{EntityName} with ID {Id} not found for deletion", _entityName, id);
                    return ApiResponse<string>.NotFound($"{_entityName} bulunamadı.");
                }

                if (SupportsSoftDelete())
                {
                    _logger.LogInformation("Performing soft delete for {EntityName} with ID: {Id}", _entityName, id);
                    PerformSoftDelete(entity);
                    _repository.Update(entity);
                }
                else
                {
                    _logger.LogInformation("Performing hard delete for {EntityName} with ID: {Id}", _entityName, id);
                    _repository.Delete(entity);
                }

                await _repository.SaveChangesAsync();
                InvalidateCache();

                _logger.LogInformation("{EntityName} with ID {Id} deleted successfully", _entityName, id);

                return ApiResponse<string>.Ok(null, $"{_entityName} başarıyla silindi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting {EntityName} with ID: {Id}", _entityName, id);
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
            string? sortDirection)
        {
            var baseKey = GenerateCacheKey(filter);
            var sortKey = string.IsNullOrEmpty(sortField) ? "" : $"sort_{sortField}_{sortDirection}";
            
            return $"{baseKey}_{sortKey}";
        }

        protected virtual string GenerateCacheKey(
            Expression<Func<TEntity, bool>>? filter,
            string? sortField,
            string? sortDirection,
            int page,
            int pageSize)
        {
            var baseKey = GenerateCacheKey(filter, sortField, sortDirection);
            var paginationKey = $"page_{page}_pageSize_{pageSize}";
            return $"{baseKey}_{paginationKey}";
        }

        protected virtual void InvalidateCache()
        {
            // Remove all cache entries for this entity type
            _cacheService.RemoveByPrefix(_cachePrefix);
            _logger.LogInformation("Cache invalidated for {EntityName}. Prefix: {CachePrefix}", _entityName, _cachePrefix);
        }

        protected abstract TKey GetIdFromUpdateDto(TUpdateDto dto);

        protected virtual bool SupportsSoftDelete() => false;

        protected virtual void PerformSoftDelete(TEntity entity) { }

        protected virtual Task<BusinessValidationResult> ValidateBusinessRulesForCreate(TCreateDto dto)
        {
            // Override in derived classes for specific business rules
            return Task.FromResult(BusinessValidationResult.Success());
        }

        protected virtual Task<BusinessValidationResult> ValidateBusinessRulesForUpdate(TUpdateDto dto, TEntity entity)
        {
            // Override in derived classes for specific business rules
            return Task.FromResult(BusinessValidationResult.Success());
        }

        private void AddBusinessContextToHttpContext(string cacheStatus, int recordCount, int page, int totalPages)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    // Add business context to HTTP context for enhanced logging
                    httpContext.Items["CacheStatus"] = cacheStatus;
                    httpContext.Items["RecordCount"] = recordCount;
                    httpContext.Items["PageInfo"] = $"P{page}/T{totalPages}";
                    
                    // Add additional context for better debugging
                    httpContext.Items["EntityName"] = _entityName;
                    httpContext.Items["CachePrefix"] = _cachePrefix;
                }
            }
            catch
            {
                // Ignore if HTTP context is not available
            }
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