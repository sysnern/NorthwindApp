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
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;
        private const string CachePrefix = "order_list_";

        public OrderService(
            IOrderRepository repo,
            IMapper mapper,
            ICacheService cacheService)
        {
            _repo = repo;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<List<OrderDTO>>> GetAllAsync()
        {
            // 1) Cache key
            var cacheKey = CachePrefix;

            // 2) Cache kontrolü
            var cached = _cacheService.Get<List<OrderDTO>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<OrderDTO>>
                    .Ok(cached, "Siparişler cache'den getirildi.");
            }

            // 3) DB'den çek
            var orders = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<OrderDTO>>(orders);

            // 4) Boş sonuçsa 404
            if (!dtoList.Any())
            {
                return ApiResponse<List<OrderDTO>>
                    .NotFound("Hiç sipariş bulunamadı.");
            }

            // 5) Cache'e yaz
            _cacheService.Set(cacheKey, dtoList);

            // 6) Başarılı listeleme
            return ApiResponse<List<OrderDTO>>
                .Ok(dtoList, "Siparişler başarıyla listelendi.");
        }

        public async Task<ApiResponse<OrderDTO>> GetByIdAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null)
            {
                return ApiResponse<OrderDTO>
                    .NotFound("Sipariş bulunamadı.");
            }

            var dto = _mapper.Map<OrderDTO>(order);
            return ApiResponse<OrderDTO>
                .Ok(dto, "Sipariş başarıyla getirildi.");
        }

        public async Task<ApiResponse<OrderDTO>> AddAsync(OrderCreateDto dto)
        {
            var entity = _mapper.Map<Order>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var createdDto = _mapper.Map<OrderDTO>(entity);
            return ApiResponse<OrderDTO>
                .Created(createdDto, "Sipariş başarıyla eklendi.");
        }

        public async Task<ApiResponse<OrderDTO>> UpdateAsync(OrderUpdateDto dto)
        {
            var order = await _repo.GetByIdAsync(dto.OrderID);
            if (order == null)
            {
                return ApiResponse<OrderDTO>
                    .NotFound("Güncellenecek sipariş bulunamadı.");
            }

            _mapper.Map(dto, order);
            _repo.Update(order);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var updatedDto = _mapper.Map<OrderDTO>(order);
            return ApiResponse<OrderDTO>
                .Ok(updatedDto, "Sipariş başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek sipariş bulunamadı.");
            }

            _repo.Delete(order);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            // 204 No Content dönerken body istemiyorsanız:
            return ApiResponse<string>
                .NoContent("Sipariş başarıyla silindi.");
        }
    }
}
