using AutoMapper;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Business.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repo;
        private readonly IMapper _mapper;

        public SupplierService(ISupplierRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<SupplierDTO>>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<SupplierDTO>>(list);

            if (!dtoList.Any())
                return ApiResponse<List<SupplierDTO>>.Fail("Tedarikçi bulunamadı.");

            return ApiResponse<List<SupplierDTO>>.SuccessResponse(dtoList, "Tedarikçiler başarıyla listelendi.");
        }

        public async Task<ApiResponse<SupplierDTO>> GetByIdAsync(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);
            if (supplier is null)
                return ApiResponse<SupplierDTO>.Fail("Tedarikçi bulunamadı.");

            var dto = _mapper.Map<SupplierDTO>(supplier);
            return ApiResponse<SupplierDTO>.SuccessResponse(dto, "Tedarikçi başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(SupplierCreateDto dto)
        {
            var supplier = _mapper.Map<Supplier>(dto);
            await _repo.AddAsync(supplier);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Tedarikçi başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(SupplierUpdateDto dto)
        {
            var supplier = await _repo.GetByIdAsync(dto.SupplierId);
            if (supplier is null)
                return ApiResponse<string>.Fail("Güncellenecek tedarikçi bulunamadı.");

            _mapper.Map(dto, supplier);
            _repo.Update(supplier);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Tedarikçi başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);
            if (supplier is null)
                return ApiResponse<string>.Fail("Silinecek tedarikçi bulunamadı.");

            _repo.Delete(supplier);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Tedarikçi başarıyla silindi.");
        }
    }
}
