using AutoMapper;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Product
            CreateMap<Product, ProductDTO>().ReverseMap();

            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();

            //Category
            CreateMap<Category, CategoryDTO>().ReverseMap();

            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>().ReverseMap();
            
            //Customer 
            CreateMap<Customer, CustomerDTO>().ReverseMap();

            CreateMap<Customer, CustomerCreateDto>().ReverseMap();
            CreateMap<Customer, CustomerUpdateDto>().ReverseMap();

            //Supplier
            CreateMap<Supplier, SupplierDTO>().ReverseMap();

            CreateMap<SupplierCreateDto, Supplier>().ReverseMap();
            CreateMap<SupplierUpdateDto, Supplier>().ReverseMap();

            //Employee
            CreateMap<Employee, EmployeeDTO>().ReverseMap();

            CreateMap<EmployeeCreateDto, Employee>().ReverseMap();
            CreateMap<EmployeeUpdateDto, Employee>().ReverseMap();

            // Order
            CreateMap<Order, OrderDTO>().ReverseMap();

            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderUpdateDto, Order>();



        }
    }
}
