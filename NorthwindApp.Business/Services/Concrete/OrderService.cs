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

        public async Task<ApiResponse<List<OrderDTO>>> GetAllAsync(OrderFilterDto? filter = null)
        {
            // 1) Filter null ise default değerler ata
            filter ??= new OrderFilterDto();

            // 2) Cache key oluştur
            var cacheKey = CachePrefix +
                $"{filter.CustomerId}_{filter.EmployeeId}_{filter.OrderDateFrom}_{filter.OrderDateTo}_{filter.IsDeleted}_{filter.SortField}_{filter.SortDirection}_{filter.Page}_{filter.PageSize}";

            // 3) Cache kontrolü - Cache'den total count da al
            var cacheKeyWithTotal = cacheKey + "_total";
            var cached = _cacheService.Get<List<OrderDTO>>(cacheKey);
            var cachedTotal = _cacheService.Get<int?>(cacheKeyWithTotal);
            
            if (cached != null && cachedTotal.HasValue)
            {
                return ApiResponse<List<OrderDTO>>
                    .Ok(cached, "Siparişler cache'den getirildi.", cachedTotal.Value, filter.Page, filter.PageSize);
            }

            // 4) DB'den çek - Önce total count hesapla
            var allOrders = await _repo.GetAllAsync(o => 
                (!filter.IsDeleted.HasValue || o.IsDeleted == filter.IsDeleted) && // Soft delete filter
                (string.IsNullOrEmpty(filter.CustomerId) || o.CustomerId.Contains(filter.CustomerId)) &&
                (!filter.EmployeeId.HasValue || o.EmployeeId == filter.EmployeeId) &&
                (!filter.OrderDateFrom.HasValue || o.OrderDate >= filter.OrderDateFrom) &&
                (!filter.OrderDateTo.HasValue || o.OrderDate <= filter.OrderDateTo)
            );

            // 5) Boş sonuçsa 404
            if (!allOrders.Any())
            {
                return ApiResponse<List<OrderDTO>>
                    .NotFound("Hiç sipariş bulunamadı.");
            }

            // 6) Sorting uygula
            var sortedOrders = allOrders.AsQueryable();
            if (!string.IsNullOrEmpty(filter.SortField))
            {
                sortedOrders = filter.SortField.ToLower() switch
                {
                    "orderid" => filter.SortDirection == "desc" ? sortedOrders.OrderByDescending(o => o.OrderId) : sortedOrders.OrderBy(o => o.OrderId),
                    "customerid" => filter.SortDirection == "desc" ? sortedOrders.OrderByDescending(o => o.CustomerId) : sortedOrders.OrderBy(o => o.CustomerId),
                    "employeeid" => filter.SortDirection == "desc" ? sortedOrders.OrderByDescending(o => o.EmployeeId) : sortedOrders.OrderBy(o => o.EmployeeId),
                    "orderdate" => filter.SortDirection == "desc" ? sortedOrders.OrderByDescending(o => o.OrderDate) : sortedOrders.OrderBy(o => o.OrderDate),
                    "isdeleted" => filter.SortDirection == "desc" ? sortedOrders.OrderByDescending(o => o.IsDeleted) : sortedOrders.OrderBy(o => o.IsDeleted),
                    _ => sortedOrders.OrderBy(o => o.OrderId)
                };
            }

            // 7) Total count hesapla (sorting'den sonra)
            var totalCount = sortedOrders.Count();

            // 8) Pagination uygula
            var pagedList = sortedOrders
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var dtoList = _mapper.Map<List<OrderDTO>>(pagedList);

            // 9) Cache'e yaz (paged result ve total count)
            _cacheService.Set(cacheKey, dtoList);
            _cacheService.Set(cacheKeyWithTotal, totalCount);

            // 10) Başarılı listeleme (total count ile birlikte)
            return ApiResponse<List<OrderDTO>>
                .Ok(dtoList, "Siparişler başarıyla listelendi.", totalCount, filter.Page, filter.PageSize);
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

        public async Task<ApiResponse<string>> AddAsync(OrderCreateDto dto)
        {
            var entity = _mapper.Map<Order>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Created("Sipariş başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(OrderUpdateDto dto)
        {
            var order = await _repo.GetByIdAsync(dto.OrderId);
            if (order == null)
            {
                return ApiResponse<string>
                    .NotFound("Güncellenecek sipariş bulunamadı.");
            }

            _mapper.Map(dto, order);
            _repo.Update(order);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .Ok("Sipariş başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek sipariş bulunamadı.");
            }

            // Soft delete - IsDeleted set et
            order.IsDeleted = true;
            _repo.Update(order);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Sipariş başarıyla silindi.");
        }
    }
}
