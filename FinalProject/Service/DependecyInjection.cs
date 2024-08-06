using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Service.DTOs.Account;
using Service.Helpers;
using Service.Services.Interfaces;
using Service.Services;
using FluentValidation.AspNetCore;

namespace Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServiceLayer(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = true;
            });

            services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddScoped<UrlHelperService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISliderService, SliderService>();
            services.AddScoped<IAboutService, AboutService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IContactSettingService, ContactSettingService>();
            services.AddScoped<ISocialService, SocialService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<ITestimonialService, TestimonialService>();
            services.AddScoped<IInstagramService, InstagramService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<ISubscribeService, SubscribeService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ISendEmail, SendEmail>();

            return services;
        }
    }
}
