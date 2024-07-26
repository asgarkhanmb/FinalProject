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


            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISliderService, SliderService>();
            services.AddScoped<IAboutService, AboutService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
