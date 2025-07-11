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

        public CustomerService(ICustomerRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApiResponse<List<CustomerDTO>>> GetAllAsync()
        {
            var customers = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<CustomerDTO>>(customers);
            return dtoList.Any()
                ? ApiResponse<List<CustomerDTO>>.SuccessResponse(dtoList)
                : ApiResponse<List<CustomerDTO>>.Fail("Hiç müşteri bulunamadı.");
        }

        public async Task<ApiResponse<CustomerDTO>> GetByIdAsync(string id)
        {
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null)
                return ApiResponse<CustomerDTO>.Fail("Müşteri bulunamadı.");

            var dto = _mapper.Map<CustomerDTO>(customer);
            return ApiResponse<CustomerDTO>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<string>> AddAsync(CustomerCreateDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            await _repo.AddAsync(customer);
            await _repo.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse(null, "Müşteri eklendi.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(CustomerUpdateDto dto)
        {
            var customer = await _repo.GetByIdAsync(dto.CustomerID);
            if (customer == null)
                return ApiResponse<string>.Fail("Müşteri bulunamadı.");

            _mapper.Map(dto, customer);
            _repo.Update(customer);
            await _repo.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse(null, "Müşteri güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(string id)
        {
            var customer = await _repo.GetByIdAsync(id);
            if (customer == null)
                return ApiResponse<string>.Fail("Müşteri bulunamadı.");

            _repo.Delete(customer);
            await _repo.SaveChangesAsync();
            return ApiResponse<string>.SuccessResponse(null, "Müşteri silindi.");
        }
    }
}
