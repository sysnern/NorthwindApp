using AutoMapper;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;

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
            // 1) Oluşturulan key
            var cacheKey = CachePrefix;

            // 2) Cache kontrolü
            var cached = _cacheService.Get<List<OrderDTO>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<OrderDTO>>
                    .SuccessResponse(cached, "Siparişler cache'den getirildi.");
            }

            // 3) DB’den çek
            var orders = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<OrderDTO>>(orders);

            if (dtoList == null || !dtoList.Any())
                return ApiResponse<List<OrderDTO>>
                    .Fail("Hiç sipariş bulunamadı.");

            // 4) Cache’e yaz
            _cacheService.Set(cacheKey, dtoList);

            return ApiResponse<List<OrderDTO>>
                .SuccessResponse(dtoList, "Siparişler başarıyla listelendi.");
        }

        public async Task<ApiResponse<OrderDTO>> GetByIdAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null)
                return ApiResponse<OrderDTO>
                    .Fail("Sipariş bulunamadı.");

            var dto = _mapper.Map<OrderDTO>(order);
            return ApiResponse<OrderDTO>
                .SuccessResponse(dto, "Sipariş başarıyla getirildi.");
        }

        public async Task<ApiResponse<string>> AddAsync(OrderCreateDto dto)
        {
            var entity = _mapper.Map<Order>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .SuccessResponse(null, "Sipariş başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(OrderUpdateDto dto)
        {
            var order = await _repo.GetByIdAsync(dto.OrderID);
            if (order == null)
                return ApiResponse<string>
                    .Fail("Güncellenecek sipariş bulunamadı.");

            _mapper.Map(dto, order);
            _repo.Update(order);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .SuccessResponse(null, "Sipariş başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null)
                return ApiResponse<string>
                    .Fail("Silinecek sipariş bulunamadı.");

            _repo.Delete(order);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .SuccessResponse(null, "Sipariş başarıyla silindi.");
        }
    }
}
