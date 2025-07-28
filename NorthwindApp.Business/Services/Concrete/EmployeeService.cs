using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            // 2) Cache kontrolü
            var cached = _cacheService.Get<List<EmployeeDTO>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<EmployeeDTO>>
                    .Ok(cached, "Çalışanlar cache'den getirildi.");
            }

            // 3) DB'den çek
            var list = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<EmployeeDTO>>(list);

            // 4) Boş sonuçsa 404
            if (!dtoList.Any())
            {
                return ApiResponse<List<EmployeeDTO>>
                    .NotFound("Hiç çalışan bulunamadı.");
            }

            // 5) Cache'e yaz
            _cacheService.Set(cacheKey, dtoList);

            // 6) Başarılı listeleme
            return ApiResponse<List<EmployeeDTO>>
                .Ok(dtoList, "Çalışanlar başarıyla listelendi.");
        }

        public async Task<ApiResponse<EmployeeDTO>> GetByIdAsync(int id)
        {
            var employee = await _repo.GetByIdAsync(id);
            if (employee == null)
            {
                return ApiResponse<EmployeeDTO>
                    .NotFound("Çalışan bulunamadı.");
            }

            var dto = _mapper.Map<EmployeeDTO>(employee);
            return ApiResponse<EmployeeDTO>
                .Ok(dto, "Çalışan başarıyla getirildi.");
        }

        public async Task<ApiResponse<EmployeeDTO>> AddAsync(EmployeeCreateDto dto)
        {
            var entity = _mapper.Map<Employee>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var createdDto = _mapper.Map<EmployeeDTO>(entity);
            return ApiResponse<EmployeeDTO>
                .Created(createdDto, "Çalışan başarıyla eklendi.");
        }

        public async Task<ApiResponse<EmployeeDTO>> UpdateAsync(EmployeeUpdateDto dto)
        {
            var employee = await _repo.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                return ApiResponse<EmployeeDTO>
                    .NotFound("Güncellenecek çalışan bulunamadı.");
            }

            _mapper.Map(dto, employee);
            _repo.Update(employee);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var updatedDto = _mapper.Map<EmployeeDTO>(employee);
            return ApiResponse<EmployeeDTO>
                .Ok(updatedDto, "Çalışan başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var employee = await _repo.GetByIdAsync(id);
            if (employee == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek çalışan bulunamadı.");
            }

            _repo.Delete(employee);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Çalışan başarıyla silindi.");
        }
    }
}
