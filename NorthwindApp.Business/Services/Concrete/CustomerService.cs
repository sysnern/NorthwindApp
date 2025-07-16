using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;

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

        public async Task<ApiResponse<List<CustomerDTO>>> GetAllAsync()
        {
            // 1) Cache key
            var cacheKey = CachePrefix;

            // 2) Cache'de var mı?
            var cached = _cacheService.Get<List<CustomerDTO>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<CustomerDTO>>
                    .SuccessResponse(cached, "Müşteriler cache'den getirildi.");
            }

            // 3) Yoksa DB'den çek
            var customers = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<CustomerDTO>>(customers);

            if (dtoList == null || !dtoList.Any())
                return ApiResponse<List<CustomerDTO>>
                    .Fail("Hiç müşteri bulunamadı.");

            // 4) Cache'e yaz
            _cacheService.Set(cacheKey, dtoList);

            return ApiResponse<List<CustomerDTO>>
                .SuccessResponse(dtoList, "Müşteriler başarıyla listelendi.");
        }

        public async Task<ApiResponse<CustomerDTO>> GetByIdAsync(string id)
        {
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null)
                return ApiResponse<CustomerDTO>
                    .Fail("Müşteri bulunamadı.");

            var dto = _mapper.Map<CustomerDTO>(customer);
            return ApiResponse<CustomerDTO>
                .SuccessResponse(dto, "Müşteri başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(CustomerCreateDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            await _repo.AddAsync(customer);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .SuccessResponse(null, "Müşteri başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(CustomerUpdateDto dto)
        {
            var customer = await _repo.GetByIdAsync(dto.CustomerID);
            if (customer == null)
                return ApiResponse<string>
                    .Fail("Güncellenecek müşteri bulunamadı.");

            _mapper.Map(dto, customer);
            _repo.Update(customer);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .SuccessResponse(null, "Müşteri başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(string id)
        {
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null)
                return ApiResponse<string>
                    .Fail("Silinecek müşteri bulunamadı.");

            _repo.Delete(customer);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .SuccessResponse(null, "Müşteri başarıyla silindi.");
        }
    }
}
