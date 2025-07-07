using AutoMapper;
using NorthwindApp.Core.DTOs;
using NorthwindApp.Entities.Models;

namespace NorthwindApp.Business.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDTO>().ReverseMap();

            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();
        }
    }
}
