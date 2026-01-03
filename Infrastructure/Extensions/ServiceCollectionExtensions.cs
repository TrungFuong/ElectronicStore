using Application.Implementations;
using Application.Interfaces;
using Domain.Entities;
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
            // bdCntext
            services.AddDbContext<DBContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // Repo
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartItemRepository, CartItemRepository>();
            services.AddScoped<IProductVariationRepository, ProductVariationRepository>();

            // Uow
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Auth
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAdminAccountService, AdminAccountService>();

            // Email (OTP)
            services.Configure<EmailSettings>(
                config.GetSection("EmailSettings"));

            services.AddScoped<IEmailService, GmailEmailService>();

            // OTP
            services.AddScoped<IOTPService, OTPService>();

            return services;
        }
    }
}
