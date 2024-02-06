using Helios.eCRF.Services.Interfaces;
using Helios.eCRF.Services;
using Helios.eCRF.Helpers;
using Helios.eCRF.Models;
using StackExchange.Redis;

namespace Helios.eCRF.Extension
{
    public static class StartupServiceExtension
    {
        public static IServiceCollection DefaultConfigurationService(this IServiceCollection services, IConfiguration Configuration)
        {
            ConfigurationOptions options = new ConfigurationOptions
            {
                EndPoints = { { Configuration["Redis:Host"], int.Parse(Configuration["Redis:Port"]) }, },
                SyncTimeout = 10 * 1000,
                AbortOnConnectFail = false,
                KeepAlive = 30,
                ConnectRetry = 10,
                Password = Configuration["Redis:Password"]
            };

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(options));

            services.AddTransient<IDatabase>(provider =>
            {
                var redis = provider.GetRequiredService<IConnectionMultiplexer>();
                return redis.GetDatabase();
            });

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStudyService, StudyService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<TimeZoneHelper>();
            return services;
        }
    }
}
