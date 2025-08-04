using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories.Abstract;
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

        public async Task<ApiResponse<List<EmployeeDTO>>> GetAllAsync(EmployeeFilterDto? filter = null)
        {
            // 1) Filter null ise default değerler ata
            filter ??= new EmployeeFilterDto();

            // Filter property'lerini local variable'lara ata
            var firstName = filter.FirstName ?? "";
            var lastName = filter.LastName ?? "";
            var title = filter.Title ?? "";
            var country = filter.Country ?? "";
            var isDeleted = filter.IsDeleted;
            var sortField = filter.SortField ?? "";
            var sortDirection = filter.SortDirection ?? "";
            var page = filter.Page;
            var pageSize = filter.PageSize;

            // 2) Cache key oluştur
            var cacheKey = CachePrefix +
                $"{firstName}_{lastName}_{title}_{country}_{isDeleted}_{sortField}_{sortDirection}_{page}_{pageSize}";

            // 3) Cache kontrolü - Cache'den total count da al
            var cacheKeyWithTotal = cacheKey + "_total";
            var cached = _cacheService.Get<List<EmployeeDTO>>(cacheKey);
            var cachedTotal = _cacheService.Get<int?>(cacheKeyWithTotal);
            
            if (cached != null && cachedTotal.HasValue)
            {
                return ApiResponse<List<EmployeeDTO>>
                    .Ok(cached, "Çalışanlar cache'den getirildi.", cachedTotal.Value, page, pageSize);
            }

            // 4) DB'den çek - Önce total count hesapla
            var allEmployees = await _repo.GetAllAsync(e => 
                (!isDeleted.HasValue || e.IsDeleted == isDeleted) && // Soft delete filter
                (string.IsNullOrEmpty(firstName) || (e.FirstName != null && e.FirstName.Contains(firstName))) &&
                (string.IsNullOrEmpty(lastName) || (e.LastName != null && e.LastName.Contains(lastName))) &&
                (string.IsNullOrEmpty(title) || (e.Title != null && e.Title.Contains(title))) &&
                (string.IsNullOrEmpty(country) || (e.Country != null && e.Country.Contains(country)))
            );

            // 5) Boş sonuçsa 404
            if (!allEmployees.Any())
            {
                return ApiResponse<List<EmployeeDTO>>
                    .NotFound("Hiç çalışan bulunamadı.");
            }

            // 6) Sorting uygula
            var sortedEmployees = allEmployees.AsQueryable();
            if (!string.IsNullOrEmpty(sortField))
            {
                var direction = sortDirection ?? "";
                sortedEmployees = sortField.ToLower() switch
                {
                    "employeeid" => direction == "desc" ? sortedEmployees.OrderByDescending(e => e.EmployeeId) : sortedEmployees.OrderBy(e => e.EmployeeId),
                    "firstname" => direction == "desc" ? sortedEmployees.OrderByDescending(e => e.FirstName) : sortedEmployees.OrderBy(e => e.FirstName),
                    "lastname" => direction == "desc" ? sortedEmployees.OrderByDescending(e => e.LastName) : sortedEmployees.OrderBy(e => e.LastName),
                    "title" => direction == "desc" ? sortedEmployees.OrderByDescending(e => e.Title) : sortedEmployees.OrderBy(e => e.Title),
                    "country" => direction == "desc" ? sortedEmployees.OrderByDescending(e => e.Country) : sortedEmployees.OrderBy(e => e.Country),
                    "isdeleted" => direction == "desc" ? sortedEmployees.OrderByDescending(e => e.IsDeleted) : sortedEmployees.OrderBy(e => e.IsDeleted),
                    _ => sortedEmployees.OrderBy(e => e.EmployeeId)
                };
            }

            // 7) Total count hesapla (sorting'den sonra)
            var totalCount = sortedEmployees.Count();

            // 8) Pagination uygula
            var pagedList = sortedEmployees
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = _mapper.Map<List<EmployeeDTO>>(pagedList);

            // 9) Cache'e yaz (paged result ve total count)
            _cacheService.Set(cacheKey, dtoList);
            _cacheService.Set(cacheKeyWithTotal, totalCount);

            // 10) Başarılı listeleme (total count ile birlikte)
            return ApiResponse<List<EmployeeDTO>>
                .Ok(dtoList, "Çalışanlar başarıyla listelendi.", totalCount, page, pageSize);
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

        public async Task<ApiResponse<string>> AddAsync(EmployeeCreateDto dto)
        {
            var entity = _mapper.Map<Employee>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Created("Çalışan başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(EmployeeUpdateDto dto)
        {
            var employee = await _repo.GetByIdAsync(dto.EmployeeId);
            if (employee == null)
            {
                return ApiResponse<string>
                    .NotFound("Güncellenecek çalışan bulunamadı.");
            }

            _mapper.Map(dto, employee);
            _repo.Update(employee);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Ok("Çalışan başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var employee = await _repo.GetByIdAsync(id);
            if (employee == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek çalışan bulunamadı.");
            }

            // Soft delete - IsDeleted set et
            employee.IsDeleted = true;
            _repo.Update(employee);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Çalışan başarıyla silindi.");
        }
    }
}
