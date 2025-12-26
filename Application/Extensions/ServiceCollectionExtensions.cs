using Application.Implementations;
using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Application-level services
            services.AddScoped<IAuthService, AuthService>();

            // Product
            services.AddScoped<IProductService, ProductService>();

            // Brand
            services.AddScoped<IBrandService, BrandService>();

            // Category
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IProductVariationService, ProductVariationService>();

            services.AddScoped<IProductSpecificationService, ProductSpecificationService>();

            services.AddScoped<IProductImageService, ProductImageService>();

            services.AddScoped<IAttributeService, AttributeService>();

            return services;
        }
    }
}
