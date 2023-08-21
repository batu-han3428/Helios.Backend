using Helios.Authentication.Contexts;
using Helios.Authentication.Entities;
using Helios.Authentication.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Helios.Authentication.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection IdentityServerSettings(this IServiceCollection services)
        {
            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            //    .AddEntityFrameworkStores<AuthenticationContext>()
            //    .AddDefaultTokenProviders()
            //    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, AuthenticationContext, Guid>>()
            //    .AddRoleStore<RoleStore<ApplicationRole, AuthenticationContext, Guid>>();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AuthenticationContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //options.Password.RequireDigit = true;

                //options.Password.RequireLowercase = true;

                //options.Password.RequiredLength = 6;

                //options.Password.RequiredUniqueChars = 2;

                //options.Password.RequireNonAlphanumeric = true;

                //options.Lockout.MaxFailedAccessAttempts = 5;

                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

                //options.User.AllowedUserNameCharacters = "abcçdefgğhıijklmnoöpqrsştuüvwxyzABCÇDEFGĞHIİJKLMNOÖPQRSTUÜVWXYZ0123456789-._@+";

                //options.User.RequireUniqueEmail = true;

                //options.SignIn.RequireConfirmedEmail = true;



                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = false;

            });

            return services;
        }

        public static IServiceCollection CookieSettings(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                //options.LoginPath = "/Account/Login";
                //options.LogoutPath = "/Account/Logout";
                //options.AccessDeniedPath = "/Account/AccessDenied";
                //options.SlidingExpiration = true;

                //options.Cookie.HttpOnly = true;

                //options.Cookie.Name = "UserCookie";

                //options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                //options.ExpireTimeSpan = TimeSpan.FromMinutes(40);



                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(60);
                options.LoginPath = "/Account/AccessDenied";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;

            });

            return services;
        }
    }
}
