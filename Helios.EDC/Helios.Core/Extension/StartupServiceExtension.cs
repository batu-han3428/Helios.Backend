using Helios.Common.Model;
using Helios.Core.Services.Interfaces;
using Helios.Core.Services;
using StackExchange.Redis;

namespace Helios.Core.Extension
{
    public static class StartupServiceExtension
    {
        public static IServiceCollection DefaultConfigurationService(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var redisOptions = Configuration.GetSection("Redis").Get<RedisOptions>();
                var connectionString = $"{redisOptions.Host}:{redisOptions.Port},password={redisOptions.Password},abortConnect=false";
                return ConnectionMultiplexer.Connect(connectionString);
            });

            services.AddScoped<ICacheService, CacheService>();

            services.AddSignalR();
            return services;
        }
    }
}
