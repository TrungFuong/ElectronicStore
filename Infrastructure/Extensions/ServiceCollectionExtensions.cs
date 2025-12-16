using Application.Interfaces;
using Domain.Interfaces;
using Infrastructure.DataAccess;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            // Register DbContext
            services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            //Product Repository
            services.AddScoped<IProductRepository, ProductRepository>();


            // Register Repositories
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register security utilities
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Brand Repository
            services.AddScoped<IBrandRepository, BrandRepository>();

            return services;
        }
    }
}
