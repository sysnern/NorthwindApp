using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Business.Services.Concrete
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<EmployeeDTO>>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<EmployeeDTO>>(list);

            if (!dtoList.Any())
                return ApiResponse<List<EmployeeDTO>>.Fail("Çalışan bulunamadı.");

            return ApiResponse<List<EmployeeDTO>>.SuccessResponse(dtoList, "Çalışanlar başarıyla listelendi.");
        }

        public async Task<ApiResponse<EmployeeDTO>> GetByIdAsync(int id)
        {
            var employee = await _repo.GetByIdAsync(id);
            if (employee is null)
                return ApiResponse<EmployeeDTO>.Fail("Çalışan bulunamadı.");

            var dto = _mapper.Map<EmployeeDTO>(employee);
            return ApiResponse<EmployeeDTO>.SuccessResponse(dto, "Çalışan başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(EmployeeCreateDto dto)
        {
            var employee = _mapper.Map<Employee>(dto);
            await _repo.AddAsync(employee);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Çalışan başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(EmployeeUpdateDto dto)
        {
            var employee = await _repo.GetByIdAsync(dto.EmployeeId);
            if (employee is null)
                return ApiResponse<string>.Fail("Güncellenecek çalışan bulunamadı.");

            _mapper.Map(dto, employee);
            _repo.Update(employee);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Çalışan başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var employee = await _repo.GetByIdAsync(id);
            if (employee is null)
                return ApiResponse<string>.Fail("Silinecek çalışan bulunamadı.");

            _repo.Delete(employee);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Çalışan başarıyla silindi.");
        }
    }
}
