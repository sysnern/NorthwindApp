using NorthwindApp.Core.Results;
using System.Linq.Expressions;

namespace NorthwindApp.Business.Services.Abstract
{
    /// <summary>
    /// Generic service interface that defines common CRUD operations
    /// Eliminates code duplication across specific service interfaces
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TDto">DTO type for responses</typeparam>
    /// <typeparam name="TCreateDto">DTO type for creation</typeparam>
    /// <typeparam name="TUpdateDto">DTO type for updates</typeparam>
    /// <typeparam name="TKey">Primary key type</typeparam>
    public interface IGenericService<TEntity, TDto, TCreateDto, TUpdateDto, TKey>
        where TEntity : class
        where TDto : class
        where TCreateDto : class
        where TUpdateDto : class
    {
        Task<ApiResponse<List<TDto>>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);
        Task<ApiResponse<TDto>> GetByIdAsync(TKey id);
        Task<ApiResponse<TDto>> AddAsync(TCreateDto dto);
        Task<ApiResponse<TDto>> UpdateAsync(TUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(TKey id);
    }
}