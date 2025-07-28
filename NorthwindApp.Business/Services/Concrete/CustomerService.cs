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

            // 2) Cache kontrolü
            var cached = _cacheService.Get<List<CustomerDTO>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<CustomerDTO>>
                    .Ok(cached, "Müşteriler cache'den getirildi.");
            }

            // 3) DB'den çek
            var customers = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<CustomerDTO>>(customers);

            // 4) Boş sonuçsa 404
            if (!dtoList.Any())
            {
                return ApiResponse<List<CustomerDTO>>
                    .NotFound("Hiç müşteri bulunamadı.");
            }

            // 5) Cache'e yaz
            _cacheService.Set(cacheKey, dtoList);

            // 6) Başarılı listeleme
            return ApiResponse<List<CustomerDTO>>
                .Ok(dtoList, "Müşteriler başarıyla listelendi.");
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

        public async Task<ApiResponse<CustomerDTO>> AddAsync(CustomerCreateDto dto)
        {
            var entity = _mapper.Map<Customer>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var createdDto = _mapper.Map<CustomerDTO>(entity);
            return ApiResponse<CustomerDTO>
                .Created(createdDto, "Müşteri başarıyla eklendi.");
        }

        public async Task<ApiResponse<CustomerDTO>> UpdateAsync(CustomerUpdateDto dto)
        {
            var customer = await _repo.GetByIdAsync(dto.CustomerID);
            if (customer == null)
            {
                return ApiResponse<CustomerDTO>
                    .NotFound("Güncellenecek müşteri bulunamadı.");
            }

            _mapper.Map(dto, customer);
            _repo.Update(customer);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var updatedDto = _mapper.Map<CustomerDTO>(customer);
            return ApiResponse<CustomerDTO>
                .Ok(updatedDto, "Müşteri başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(string id)
        {
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek müşteri bulunamadı.");
            }

            _repo.Delete(customer);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Müşteri başarıyla silindi.");
        }
    }
}
