using AutoMapper;
using NorthwindApp.Business.Services;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<ProductDTO>> GetAllAsync(ProductFilterDto filter)
        {
            var products = await _repo.GetAllAsync(p =>
                (string.IsNullOrEmpty(filter.ProductName) || p.ProductName.Contains(filter.ProductName)) &&
                (!filter.CategoryId.HasValue || p.CategoryId == filter.CategoryId) &&                      
                (!filter.MinPrice.HasValue || p.UnitPrice >= filter.MinPrice) &&
                (!filter.MaxPrice.HasValue || p.UnitPrice <= filter.MaxPrice) &&
                (!filter.Discontinued.HasValue || p.Discontinued == filter.Discontinued)
            );

            return _mapper.Map<List<ProductDTO>>(products);
        }

        public async Task<ProductDTO?> GetByIdAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDTO>(product);
        }

        public async Task AddAsync(ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);
            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProductUpdateDto dto)
        {
            var product = await _repo.GetByIdAsync(dto.ProductId);
            if (product is null) throw new Exception("Product not found.");

            _mapper.Map(dto, product);
            _repo.Update(product);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _repo.GetByIdAsync(id);
            if (product is null)
                throw new Exception("Product not found.");

            product.Discontinued = true; // Soft delete
            _repo.Update(product);
            await _repo.SaveChangesAsync();
        }

    }
}
