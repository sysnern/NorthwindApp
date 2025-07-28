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

        public async Task<ApiResponse<List<SupplierDTO>>> GetAllAsync()
        {
            // 1) Cache key
            var cacheKey = CachePrefix;

            // 2) Cache kontrolü
            var cached = _cacheService.Get<List<SupplierDTO>>(cacheKey);
            if (cached != null)
            {
                return ApiResponse<List<SupplierDTO>>
                    .Ok(cached, "Tedarikçiler cache'den getirildi.");
            }

            // 3) DB'den çek
            var list = await _repo.GetAllAsync();
            var dtoList = _mapper.Map<List<SupplierDTO>>(list);

            // 4) Sonuç yoksa 404
            if (!dtoList.Any())
            {
                return ApiResponse<List<SupplierDTO>>
                    .NotFound("Hiç tedarikçi bulunamadı.");
            }

            // 5) Cache'e yaz
            _cacheService.Set(cacheKey, dtoList);

            // 6) Başarılı listeleme
            return ApiResponse<List<SupplierDTO>>
                .Ok(dtoList, "Tedarikçiler başarıyla listelendi.");
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

        public async Task<ApiResponse<SupplierDTO>> AddAsync(SupplierCreateDto dto)
        {
            var entity = _mapper.Map<Supplier>(dto);
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var createdDto = _mapper.Map<SupplierDTO>(entity);
            return ApiResponse<SupplierDTO>
                .Created(createdDto, "Tedarikçi başarıyla eklendi.");
        }

        public async Task<ApiResponse<SupplierDTO>> UpdateAsync(SupplierUpdateDto dto)
        {
            var supplier = await _repo.GetByIdAsync(dto.SupplierId);
            if (supplier == null)
            {
                return ApiResponse<SupplierDTO>
                    .NotFound("Güncellenecek tedarikçi bulunamadı.");
            }

            _mapper.Map(dto, supplier);
            _repo.Update(supplier);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            var updatedDto = _mapper.Map<SupplierDTO>(supplier);
            return ApiResponse<SupplierDTO>
                .Ok(updatedDto, "Tedarikçi başarıyla güncellendi.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(int id)
        {
            var supplier = await _repo.GetByIdAsync(id);
            if (supplier == null)
            {
                return ApiResponse<string>
                    .NotFound("Silinecek tedarikçi bulunamadı.");
            }

            _repo.Delete(supplier);
            await _repo.SaveChangesAsync();

            // Cache temizle
            _cacheService.RemoveByPrefix(CachePrefix);

            return ApiResponse<string>
                .NoContent("Tedarikçi başarıyla silindi.");
        }
    }
}
