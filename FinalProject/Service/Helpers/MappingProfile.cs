using AutoMapper;
using Domain.Entities;
using Service.DTOs.Account;
using Service.DTOs.Admin.Abouts;
using Service.DTOs.Admin.Blogs;
using Service.DTOs.Admin.Categories;
using Service.DTOs.Admin.ContactSettings;
using Service.DTOs.Admin.Instagrams;
using Service.DTOs.Admin.Products;
using Service.DTOs.Admin.Settings;
using Service.DTOs.Admin.Sliders;
using Service.DTOs.Admin.Socials;
using Service.DTOs.Admin.Teams;
using Service.DTOs.Admin.Testimonials;
using Service.DTOs.Ui.Baskets;
using Service.DTOs.Ui.Contacts;
using Service.DTOs.Ui.Subscribes;
using Service.DTOs.Ui.Wishlists;

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

            CreateMap<Contact, ContactDto>();
            CreateMap<ContactCreateDto, Contact>();

            CreateMap<ContactSetting, ContactSettingDto>();
            CreateMap<ContactSettingCreateDto, ContactSetting>();
            CreateMap<ContactSettingEditDto, ContactSetting>().ForMember(dest => dest.Image, opt => opt.Condition(src => (src.Image is not null)));

            CreateMap<Social, SocialDto>();
            CreateMap<SocialCreateDto, Social>();
            CreateMap<SocialEditDto, Social>();

            CreateMap<Team, TeamDto>().ForMember(dest => dest.SocialNames, opt => opt.MapFrom(src => src.Socials.Select(m=>m.Name).ToList()));
            CreateMap<TeamCreateDto, Team>();
            CreateMap<TeamEditDto, Team>().ForMember(dest => dest.Image, opt => opt.Condition(src => (src.Image is not null)));

            CreateMap<Testimonial, TestimonialDto>();
            CreateMap<TestimonialCreateDto, Testimonial>();
            CreateMap<TestimonialEditDto, Testimonial>().ForMember(dest => dest.Image, opt => opt.Condition(src => (src.Image is not null)));

            CreateMap<Instagram, InstagramDto>().ForMember(d => d.Images, opt => opt.MapFrom(s => s.InstagramGalleries.Select(m => m.Image).ToList()));
            CreateMap<InstagramCreateDto, Instagram>();
            CreateMap<InstagramEditDto, Instagram>().ForMember(dest => dest.InstagramGalleries, opt => opt.Condition(src => (src.InstagramGalleries is not null)));

            CreateMap<Blog, BlogDto>();
            CreateMap<BlogCreateDto, Blog>();
            CreateMap<BlogEditDto, Blog>().ForMember(dest => dest.Image, opt => opt.Condition(src => (src.Image is not null)));

            CreateMap<Setting, SettingDto>();
            CreateMap<SettingCreateDto, Setting>();
            CreateMap<SettingEditDto, Setting>().ForMember(dest => dest.Logo, opt => opt.Condition(src => (src.Logo is not null)));

            CreateMap<Subscribe, SubscribeDto>();
            CreateMap<SubscribeCreateDto, Subscribe>();

            CreateMap<Wishlist, WishlistDto>();
            CreateMap<WishlistDto, Wishlist>();
            CreateMap<WishlistProduct, WishlistProductDto>();
            CreateMap<WishlistProductDto, WishlistProduct>();

            CreateMap<Basket, BasketDto>();
            CreateMap<BasketCreateDto, Basket>();
            CreateMap<BasketProduct, BasketProductDto>();
            CreateMap<BasketProductDto, BasketProduct>();



        }
    }
}
