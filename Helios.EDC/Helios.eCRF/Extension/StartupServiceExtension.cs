﻿using Helios.eCRF.Services.Interfaces;
using Helios.eCRF.Services;
using Helios.eCRF.Helpers;

namespace Helios.eCRF.Extension
{
    public static class StartupServiceExtension
    {
        private static IConfiguration Configuration;
        public static IServiceCollection DefaultConfigurationService(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStudyService, StudyService>();
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<TimeZoneHelper>();

            return services;
        }

    }
}
