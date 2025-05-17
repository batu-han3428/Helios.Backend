using Helios.Authentication.Consumers;
using Helios.Authentication.Contexts;
using Helios.Authentication.Helpers;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using Helios.Authentication.Services.Interfaces;
using Helios.Authentication.Services;
using Helios.Authentication.Domains.Entities;

namespace Helios.Authentication.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection IdentityServerSettings(this IServiceCollection services, IConfiguration Configuration)
        {
            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            //    .AddEntityFrameworkStores<AuthenticationContext>()
            //    .AddDefaultTokenProviders()
            //    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, AuthenticationContext, Guid>>()
            //    .AddRoleStore<RoleStore<ApplicationRole, AuthenticationContext, Guid>>();

            var activeSql = Configuration["AppConfig:ActiveSql"];
            var conn = $"{activeSql}_DefaultConnection";

            services.ConfigureMysqlPooled<AuthenticationContext>(Configuration, connectionString: conn, MigrationsAssembly: "Helios.Authentication");

            services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AuthenticationContext>()
            .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                //options.Password.RequiredUniqueChars = 2;

                //options.User.AllowedUserNameCharacters = "abcçdefgğhıijklmnoöpqrsştuüvwxyzABCÇDEFGĞHIİJKLMNOÖPQRSTUÜVWXYZ0123456789-._@+";

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


                //options.Cookie.Domain = "localhost:44458";
                //options.Cookie.Path = "localhost:44458";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(60);
                //options.LoginPath = "/Account/AccessDenied";
                //options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;


                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
                options.Cookie.Name = "UserCookie";
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;

                options.LoginPath = new PathString("/");
                options.LogoutPath = new PathString("/");
                options.AccessDeniedPath = new PathString("/");



                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents()
                {
                    OnRedirectToAccessDenied = ctx =>
                    {
                        ctx.Response.Redirect("/Login/UserAccessDenied?code=401");
                        return Task.CompletedTask;
                    }
                };

            });

            return services;
        }

        public static void ConfigurationEventBusConsumers(this IServiceCollection services, IConfiguration Configuration, IWebHostEnvironment env)
        {
            try
            {
                string qPrefix = "";

                if (env.IsDevelopment())
                {
                    qPrefix = "localhost_";
                }

                qPrefix = qPrefix + Configuration["AppConfig:QueuePrefix"] + "_activesql_" + Configuration["AppConfig:ActiveSql"];

                var rabbitMQUrl = Configuration["AppConfig:RabbitMQCrf"];
                var rabbitMQUserName = Configuration["AppConfig:RabbitMQUserName"];
                var rabbitMQPassword = Configuration["AppConfig:RabbitMQPassword"];
                
                var rabbitMQueueName_UserConsumer = qPrefix + Configuration["AppConfig:RabbitMQueueName_UserConsumer"];

                var rabbitMQueueName_SystemAdminUserConsumer = qPrefix + Configuration["AppConfig:RabbitMQueueName_SystemAdminUserConsumer"];

                var rabbitMQueueName_UserResetPassword = qPrefix + Configuration["AppConfig:RabbitMQueueName_UserResetPassword"];

                var rabbitMQueueName_ForgotPassword = qPrefix + Configuration["AppConfig:RabbitMQueueName_ForgotPassword"];

                var rabbitMQueueName_CreateRoleUser = qPrefix + Configuration["AppConfig:RabbitMQueueName_CreateRoleUser"]; 

                services.AddScoped<AddStudyUserConsumer>();
                services.AddScoped<AddSystemAdminUserConsumer>();
                services.AddScoped<UserResetPasswordConsumer>();
                services.AddScoped<ForgotPasswordConsumer>();
                services.AddScoped<CreateRoleUserConsumer>();
                
                services.AddMassTransit(c =>
                {
                    c.AddConsumer<AddStudyUserConsumer>();
                    c.AddConsumer<AddSystemAdminUserConsumer>();
                    c.AddConsumer<UserResetPasswordConsumer>();
                    c.AddConsumer<ForgotPasswordConsumer>();
                    c.AddConsumer<CreateRoleUserConsumer>();
                    
                    c.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                            cfg.Host(new Uri(rabbitMQUrl),
                               b => { b.Username(rabbitMQUserName); b.Password(rabbitMQPassword); });

                        cfg.ReceiveEndpoint(rabbitMQueueName_UserConsumer, ep =>
                        {
                            ep.PrefetchCount = 16;
                            ep.UseMessageRetry(r => r.Interval(2, 100));
                            ep.ConfigureConsumer<AddStudyUserConsumer>(provider);
                        });

                        cfg.ReceiveEndpoint(rabbitMQueueName_SystemAdminUserConsumer, ep =>
                        {
                            ep.PrefetchCount = 16;
                            ep.UseMessageRetry(r => r.Interval(2, 100));
                            ep.ConfigureConsumer<AddSystemAdminUserConsumer>(provider);
                        });

                        cfg.ReceiveEndpoint(rabbitMQueueName_UserResetPassword, ep =>
                        {
                            ep.PrefetchCount = 16;
                            ep.UseMessageRetry(r => r.Interval(2, 100));
                            ep.ConfigureConsumer<UserResetPasswordConsumer>(provider);
                        });

                        cfg.ReceiveEndpoint(rabbitMQueueName_ForgotPassword, ep =>
                        {
                            ep.PrefetchCount = 16;
                            ep.UseMessageRetry(r => r.Interval(2, 100));
                            ep.ConfigureConsumer<ForgotPasswordConsumer>(provider);
                        });
                    
                        cfg.ReceiveEndpoint(rabbitMQueueName_CreateRoleUser, ep =>
                        {
                            ep.PrefetchCount = 16;
                            ep.UseMessageRetry(r => r.Interval(2, 100));
                            ep.ConfigureConsumer<CreateRoleUserConsumer>(provider);
                        });
                    }));
                });
            }
            catch (Exception e)
            {

                throw;
            }
           
        }

        public static void ConfigurationEventBusJob(this IServiceCollection services)
        {
            try
            {
                services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
                services.AddSingleton<IHostedService, Helios.EventBus.Base.MassTransitBusHostedService>();
            }
            catch (Exception e)
            {

                throw;
            }
        }
    
        public static void SmtpSettings(this IServiceCollection services)
        {
            services.AddScoped<SmtpClient>
               ((serviceProvider) =>
               {
                   var config = serviceProvider.GetRequiredService<IConfiguration>();

                   var host = config["EmailSender:Host"];
                   var port = config.GetValue<int>("EmailSender:Port");
                   var enableSSL = config.GetValue<bool>("EmailSender:EnableSSL");
                   var userName = config["EmailSender:UserName"];
                   var password = config["EmailSender:Password"];
                   var client = new SmtpClient(host, port)
                   {
                       Credentials = new NetworkCredential(userName, password),
                       EnableSsl = enableSSL
                   };

                   return client;

               });
        }
    
        public static void DependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IBaseService, BaseService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IApiBaseService, ApiBaseService>();
            services.AddScoped<ICoreService, CoreService>();
            services.AddScoped<IFileStorageHelper, AzureBlobHelper>();
            services.AddScoped<ITokenHandler, TokenHandler>();
            services.AddScoped<ITimeZoneHelper, TimeZoneHelper>();
        }
    }
}
