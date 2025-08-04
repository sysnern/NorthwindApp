using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services.Abstract
{
    public interface ISupplierService
    {
        Task<ApiResponse<List<SupplierDTO>>> GetAllAsync(SupplierFilterDto? filter = null);
        Task<ApiResponse<SupplierDTO>> GetByIdAsync(int id);
        Task<ApiResponse<string>> AddAsync(SupplierCreateDto dto);
        Task<ApiResponse<string>> UpdateAsync(SupplierUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
