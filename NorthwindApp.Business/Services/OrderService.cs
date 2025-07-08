using AutoMapper;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<OrderDTO>>> GetAllAsync()
        {
            var orders = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<OrderDTO>>(orders);

            if (!dtoList.Any())
                return ApiResponse<List<OrderDTO>>.Fail("Sipariş bulunamadı.");

            return ApiResponse<List<OrderDTO>>.SuccessResponse(dtoList);
        }

        public async Task<ApiResponse<OrderDTO>> GetByIdAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order is null)
                return ApiResponse<OrderDTO>.Fail("Sipariş bulunamadı.");

            var dto = _mapper.Map<OrderDTO>(order);
            return ApiResponse<OrderDTO>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<string>> AddAsync(OrderCreateDto dto)
        {
            var entity = _mapper.Map<Order>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Sipariş başarıyla eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(OrderUpdateDto dto)
        {
            var order = await _repo.GetByIdAsync(dto.OrderID);
            if (order == null)
                return ApiResponse<string>.Fail("Güncellenecek sipariş bulunamadı.");

            _mapper.Map(dto, order);
            _repo.Update(order);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Sipariş başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var order = await _repo.GetByIdAsync(id);
            if (order == null)
                return ApiResponse<string>.Fail("Silinecek sipariş bulunamadı.");

            _repo.Delete(order);
            await _repo.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse(null, "Sipariş başarıyla silindi.");
        }
    }
}
