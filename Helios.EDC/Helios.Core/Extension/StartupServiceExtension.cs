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

            services.AddScoped<IUserService, UserService>();

            services.AddSignalR();
            return services;
        }
    }
}
