using Microsoft.Extensions.DependencyInjection;
using Repository.Repositories.Interfaces;
using Repository.Repositories;


namespace Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<ISliderRepository, SliderRepository>();
            services.AddScoped<IAboutRepository, AboutRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IContactSettingRepository, ContactSettingRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<ISocialRepository, SocialRepository>();
            services.AddScoped<ITestimonialRepository,TestimonialRepository>();
            services.AddScoped<IInstagramRepository, InstagramRepository>();



            return services;
        }
    }
}
