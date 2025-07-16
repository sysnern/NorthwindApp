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
        private readonly ICacheService _cacheService;
        private const string CachePrefix = "employee_list_";

        public EmployeeService(
            IEmployeeRepository repo,
            IMapper mapper,
            ICacheService cacheService)
        {
            _repo = repo;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<List<EmployeeDTO>>> GetAllAsync()
        {
            // 1) Cache key
            var cacheKey = CachePrefix;

            // 2) Cache'de var mı?
            var cached = _cacheService.Get<List<EmployeeDTO>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<EmployeeDTO>>
                    .SuccessResponse(cached, "Çalışanlar cache'den getirildi.");
            }

            // 3) Yoksa DB'den çek
            var list = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<EmployeeDTO>>(list);

            if (dtoList == null || !dtoList.Any())
                return ApiResponse<List<EmployeeDTO>>
                    .Fail("Hiç çalışan bulunamadı.");

            // 4) Cache'e yaz
            _cacheService.Set(cacheKey, dtoList);

            return ApiResponse<List<EmployeeDTO>>
                .SuccessResponse(dtoList, "Çalışanlar başarıyla listelendi.");
        }

        public async Task<ApiResponse<EmployeeDTO>> GetByIdAsync(int id)
        {
            var employee = await _repo.GetByIdAsync(id);
            if (employee == null)
                return ApiResponse<EmployeeDTO>
                    .Fail("Çalışan bulunamadı.");

            var dto = _mapper.Map<EmployeeDTO>(employee);
            return ApiResponse<EmployeeDTO>
                .SuccessResponse(dto, "Çalışan başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(EmployeeCreateDto dto)
        {
            var employee = _mapper.Map<Employee>(dto);
            await _repo.AddAsync(employee);
            await _repo.SaveChangesAsync();

            //  Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .SuccessResponse(null, "Çalışan başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(EmployeeUpdateDto dto)
        {
            var employee = await _repo.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
                return ApiResponse<string>
                    .Fail("Güncellenecek çalışan bulunamadı.");

            _mapper.Map(dto, employee);
            _repo.Update(employee);
            await _repo.SaveChangesAsync();

            //  Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .SuccessResponse(null, "Çalışan başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var employee = await _repo.GetByIdAsync(id);
            if (employee == null)
                return ApiResponse<string>
                    .Fail("Silinecek çalışan bulunamadı.");

            _repo.Delete(employee);
            await _repo.SaveChangesAsync();

            //  Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .SuccessResponse(null, "Çalışan başarıyla silindi.");
        }
    }
}
