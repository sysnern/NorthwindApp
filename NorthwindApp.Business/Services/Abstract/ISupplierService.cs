using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;

namespace NorthwindApp.Business.Services.Abstract
{
    public interface ISupplierService
    {
        Task<ApiResponse<List<SupplierDTO>>> GetAllAsync();
        Task<ApiResponse<SupplierDTO>> GetByIdAsync(int id);
        Task<ApiResponse<SupplierDTO>> AddAsync(SupplierCreateDto dto);
        Task<ApiResponse<SupplierDTO>> UpdateAsync(SupplierUpdateDto dto);
        Task<ApiResponse<string>> DeleteAsync(int id);
    }
}
