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
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repo;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CachePrefix = "customer_list_";

        public CustomerService(
            ICustomerRepository repo,
            IMapper mapper,
            ICacheService cacheService)
        {
            _repo = repo;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<List<CustomerDTO>>> GetAllAsync(CustomerFilterDto? filter = null)
        {
            // 1) Filter null ise default değerler ata
            filter ??= new CustomerFilterDto();

            // Filter property'lerini local variable'lara ata
            var customerId = filter.CustomerId ?? "";
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
            var cached = _cacheService.Get<List<CustomerDTO>>(cacheKey);
            var cachedTotal = _cacheService.Get<int?>(cacheKeyWithTotal);
            
            if (cached != null && cachedTotal.HasValue)
            {
                return ApiResponse<List<CustomerDTO>>
                    .Ok(cached, "Müşteriler cache'den getirildi.", cachedTotal.Value, page, pageSize);
            }

            // 4) DB'den çek - Önce total count hesapla
            var allCustomers = await _repo.GetAllAsync(c => 
                (!isDeleted.HasValue || c.IsDeleted == isDeleted) && // Soft delete filter
                (string.IsNullOrEmpty(customerId) || (c.CustomerId != null && c.CustomerId.Contains(customerId))) &&
                (string.IsNullOrEmpty(companyName) || (c.CompanyName != null && c.CompanyName.Contains(companyName))) &&
                (string.IsNullOrEmpty(contactName) || (c.ContactName != null && c.ContactName.Contains(contactName))) &&
                (string.IsNullOrEmpty(city) || (c.City != null && c.City.Contains(city))) &&
                (string.IsNullOrEmpty(country) || (c.Country != null && c.Country.Contains(country)))
            );

            // 5) Boş sonuçsa 404
            if (!allCustomers.Any())
            {
                return ApiResponse<List<CustomerDTO>>
                    .NotFound("Hiç müşteri bulunamadı.");
            }

            // 6) Sorting uygula
            var sortedCustomers = allCustomers.AsQueryable();
            if (!string.IsNullOrEmpty(sortField))
            {
                var direction = sortDirection ?? "";
                sortedCustomers = sortField.ToLower() switch
                {
                    "customerid" => direction == "desc" ? sortedCustomers.OrderByDescending(c => c.CustomerId) : sortedCustomers.OrderBy(c => c.CustomerId),
                    "companyname" => direction == "desc" ? sortedCustomers.OrderByDescending(c => c.CompanyName) : sortedCustomers.OrderBy(c => c.CompanyName),
                    "contactname" => direction == "desc" ? sortedCustomers.OrderByDescending(c => c.ContactName) : sortedCustomers.OrderBy(c => c.ContactName),
                    "city" => direction == "desc" ? sortedCustomers.OrderByDescending(c => c.City) : sortedCustomers.OrderBy(c => c.City),
                    "country" => direction == "desc" ? sortedCustomers.OrderByDescending(c => c.Country) : sortedCustomers.OrderBy(c => c.Country),
                    "isdeleted" => direction == "desc" ? sortedCustomers.OrderByDescending(c => c.IsDeleted) : sortedCustomers.OrderBy(c => c.IsDeleted),
                    _ => sortedCustomers.OrderBy(c => c.CustomerId)
                };
            }

            // 7) Total count hesapla (sorting'den sonra)
            var totalCount = sortedCustomers.Count();

            // 8) Pagination uygula
            var pagedList = sortedCustomers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = _mapper.Map<List<CustomerDTO>>(pagedList);

            // 9) Cache'e yaz (paged result ve total count)
            _cacheService.Set(cacheKey, dtoList);
            _cacheService.Set(cacheKeyWithTotal, totalCount);

            // 10) Başarılı listeleme (total count ile birlikte)
            return ApiResponse<List<CustomerDTO>>
                .Ok(dtoList, "Müşteriler başarıyla listelendi.", totalCount, page, pageSize);
        }

        public async Task<ApiResponse<CustomerDTO>> GetByIdAsync(string id)
        {
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null)
            {
                return ApiResponse<CustomerDTO>
                    .NotFound("Müşteri bulunamadı.");
            }

            var dto = _mapper.Map<CustomerDTO>(customer);
            return ApiResponse<CustomerDTO>
                .Ok(dto, "Müşteri başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(CustomerCreateDto dto)
        {
            var entity = _mapper.Map<Customer>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Created("Müşteri başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(CustomerUpdateDto dto)
        {
            var customer = await _repo.GetByIdAsync(dto.CustomerId);
            if (customer == null)
            {
                return ApiResponse<string>
                    .NotFound("Güncellenecek müşteri bulunamadı.");
            }

            _mapper.Map(dto, customer);
            _repo.Update(customer);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Ok("Müşteri başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(string id)
        {
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek müşteri bulunamadı.");
            }

            // Soft delete - IsDeleted set et
            customer.IsDeleted = true;
            _repo.Update(customer);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Müşteri başarıyla silindi.");
        }
    }
}
