using Helios.eCRF.Models;
using StackExchange.Redis;

namespace Helios.Shared.Extension
{
    public static class StartupServiceExtension
    {
        public static IServiceCollection SharedDefaultConfigurationService(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var redisOptions = Configuration.GetSection("Redis").Get<RedisOptions>();
                var connectionString = $"{redisOptions.Host}:{redisOptions.Port},password={redisOptions.Password},abortConnect=false";
                return ConnectionMultiplexer.Connect(connectionString);
            });

            services.AddSignalR();
            return services;
        }
    }
}
