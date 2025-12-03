using Application.Implementations;
using Application.Interfaces;
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

            return services;
        }
    }
}
