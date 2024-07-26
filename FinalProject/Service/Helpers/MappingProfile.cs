using AutoMapper;
using Domain.Entities;
using Service.DTOs.Account;
using Service.DTOs.Admin.Abouts;
using Service.DTOs.Admin.Categories;
using Service.DTOs.Admin.Products;
using Service.DTOs.Admin.Sliders;
using System.Text.RegularExpressions;

namespace Service.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDto, AppUser>();
            CreateMap<AppUser, UserDto>();

            CreateMap<Slider, SliderDto>();
            CreateMap<SliderCreateDto, Slider>();
            CreateMap<SliderEditDto, Slider>().ForMember(dest => dest.Image, opt => opt.Condition(src => (src.Image is not null)));

            CreateMap<About, AboutDto>();
            CreateMap<AboutCreateDto, About>();
            CreateMap<AboutEditDto, About>().ForMember(dest => dest.Image, opt => opt.Condition(src => (src.Image is not null)));

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryEditDto, Category>().ForMember(dest => dest.Icon, opt => opt.Condition(src => (src.Icon is not null)));


            CreateMap<Product, ProductDto>().ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))

                .ForMember(d => d.Images, opt => opt.MapFrom(s => s.ProductImages.Select(m => m.Image).ToList()));
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductEditDto, Product>();
        }
    }
}
