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
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repo;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CachePrefix = "supplier_list_";

        public SupplierService(
            ISupplierRepository repo,
            IMapper mapper,
            ICacheService cacheService)
        {
            _repo = repo;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<List<SupplierDTO>>> GetAllAsync(SupplierFilterDto? filter = null)
        {
            // 1) Filter null ise default değerler ata
            filter ??= new SupplierFilterDto();

            // Filter property'lerini local variable'lara ata
            var companyName = filter.CompanyName ?? "";
            var contactName = filter.ContactName ?? "";
            var city = filter.City ?? "";
            var country = filter.Country ?? "";
            var isDeleted = filter.IsDeleted;
            var sortField = filter.SortField ?? "";
            var sortDirection = filter.SortDirection ?? "";
            var page = filter.Page;
            var pageSize = filter.PageSize;

            // 2) Cache key oluştur
            var cacheKey = CachePrefix +
                $"{companyName}_{contactName}_{city}_{country}_{isDeleted}_{sortField}_{sortDirection}_{page}_{pageSize}";

            // 3) Cache kontrolü - Cache'den total count da al
            var cacheKeyWithTotal = cacheKey + "_total";
            var cached = _cacheService.Get<List<SupplierDTO>>(cacheKey);
            var cachedTotal = _cacheService.Get<int?>(cacheKeyWithTotal);
            
            if (cached != null && cachedTotal.HasValue)
            {
                return ApiResponse<List<SupplierDTO>>
                    .Ok(cached, "Tedarikçiler cache'den getirildi.", cachedTotal.Value, page, pageSize);
            }

            // 4) DB'den çek - Önce total count hesapla
            var allSuppliers = await _repo.GetAllAsync(s => 
                (!isDeleted.HasValue || s.IsDeleted == isDeleted) && // Soft delete filter
                (string.IsNullOrEmpty(companyName) || (s.CompanyName != null && s.CompanyName.Contains(companyName))) &&
                (string.IsNullOrEmpty(contactName) || (s.ContactName != null && s.ContactName.Contains(contactName))) &&
                (string.IsNullOrEmpty(city) || (s.City != null && s.City.Contains(city))) &&
                (string.IsNullOrEmpty(country) || (s.Country != null && s.Country.Contains(country)))
            );

            // 5) Boş sonuçsa 404
            if (!allSuppliers.Any())
            {
                return ApiResponse<List<SupplierDTO>>
                    .NotFound("Hiç tedarikçi bulunamadı.");
            }

            // 6) Sorting uygula
            var sortedSuppliers = allSuppliers.AsQueryable();
            if (!string.IsNullOrEmpty(sortField))
            {
                var direction = sortDirection ?? "";
                sortedSuppliers = sortField.ToLower() switch
                {
                    "supplierid" => direction == "desc" ? sortedSuppliers.OrderByDescending(s => s.SupplierId) : sortedSuppliers.OrderBy(s => s.SupplierId),
                    "companyname" => direction == "desc" ? sortedSuppliers.OrderByDescending(s => s.CompanyName) : sortedSuppliers.OrderBy(s => s.CompanyName),
                    "contactname" => direction == "desc" ? sortedSuppliers.OrderByDescending(s => s.ContactName) : sortedSuppliers.OrderBy(s => s.ContactName),
                    "city" => direction == "desc" ? sortedSuppliers.OrderByDescending(s => s.City) : sortedSuppliers.OrderBy(s => s.City),
                    "country" => direction == "desc" ? sortedSuppliers.OrderByDescending(s => s.Country) : sortedSuppliers.OrderBy(s => s.Country),
                    "isdeleted" => direction == "desc" ? sortedSuppliers.OrderByDescending(s => s.IsDeleted) : sortedSuppliers.OrderBy(s => s.IsDeleted),
                    _ => sortedSuppliers.OrderBy(s => s.SupplierId)
                };
            }

            // 7) Total count hesapla (sorting'den sonra)
            var totalCount = sortedSuppliers.Count();

            // 8) Pagination uygula
            var pagedList = sortedSuppliers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = _mapper.Map<List<SupplierDTO>>(pagedList);

            // 9) Cache'e yaz (paged result ve total count)
            _cacheService.Set(cacheKey, dtoList);
            _cacheService.Set(cacheKeyWithTotal, totalCount);

            // 10) Başarılı listeleme (total count ile birlikte)
            return ApiResponse<List<SupplierDTO>>
                .Ok(dtoList, "Tedarikçiler başarıyla listelendi.", totalCount, page, pageSize);
        }

        public async Task<ApiResponse<SupplierDTO>> GetByIdAsync(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);
            if (supplier == null)
            {
                return ApiResponse<SupplierDTO>
                    .NotFound("Tedarikçi bulunamadı.");
            }

            var dto = _mapper.Map<SupplierDTO>(supplier);
            return ApiResponse<SupplierDTO>
                .Ok(dto, "Tedarikçi başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(SupplierCreateDto dto)
        {
            var entity = _mapper.Map<Supplier>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Created("Tedarikçi başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(SupplierUpdateDto dto)
        {
            var supplier = await _repo.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
            {
                return ApiResponse<string>
                    .NotFound("Güncellenecek tedarikçi bulunamadı.");
            }

            _mapper.Map(dto, supplier);
            _repo.Update(supplier);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Ok("Tedarikçi başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);
            if (supplier == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek tedarikçi bulunamadı.");
            }

            // Soft delete - IsDeleted set et
            supplier.IsDeleted = true;
            _repo.Update(supplier);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Tedarikçi başarıyla silindi.");
        }
    }
}
