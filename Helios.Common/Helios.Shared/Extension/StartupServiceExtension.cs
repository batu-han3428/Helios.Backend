using Helios.Common.Model;
using Helios.Shared.Services;
using Helios.Shared.Services.Interfaces;
using StackExchange.Redis;

namespace Helios.Shared.Extension
{
    public static class StartupServiceExtension
    {
        public static IServiceCollection DefaultConfigurationService(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddHttpContextAccessor();

            services.AddMemoryCache();

            services.AddScoped<ICacheService, CacheService>();

            return services;
        }
    }
}
