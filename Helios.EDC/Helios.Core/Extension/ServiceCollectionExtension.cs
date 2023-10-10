using Helios.Core.Helpers;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using Helios.Core.Contexts;
using MassTransit;
using Helios.Core.Services;
using Helios.Core.Services.Interfaces;

namespace Helios.Core.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection IdentityServerSettings(this IServiceCollection services, IConfiguration Configuration)
        {
            var activeSql = Configuration["AppConfig:ActiveSql"];
            var conn = $"{activeSql}_DefaultConnection";

            services.ConfigureMysqlPooled<CoreContext>(Configuration, connectionString: conn, MigrationsAssembly: "Helios.Authentication");

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


                var termofUseSignedConsumer = qPrefix + Configuration["AppConfig:RabbitTermOfUseSignedQueueName"];

                var mailQueueNameAdversEventPdfMailConsumer = qPrefix + Configuration["AppConfig:QueueName_AdversEventPdfMailConsumer"];

                var mailQueueUserResearchMailConsumer = qPrefix + Configuration["AppConfig:QueueName_UserResearchMailConsumer"];

                var mailQueueArchivedMailConsumer = qPrefix + Configuration["AppConfig:QueueName_ArchivedMailConsumer"];

                var mailQueueCycleMailConsumer = qPrefix + Configuration["AppConfig:QueueName_CycleMailConsumer"];

                var mailQueueMultiCycleMailConsumer = qPrefix + Configuration["AppConfig:QueueName_MultiCycleMailConsumer"];

                var moduleDataStatusQueueName = qPrefix + Configuration["AppConfig:RabbitMQueueName_ModuleDataStatus"];

                var mailQueueUserInputMailConsumer = qPrefix + Configuration["AppConfig:RabbitMQueueName_InputMails"];

                var rabbitMQueueName_TmfMail = qPrefix + Configuration["AppConfig:RabbitMQueueName_TmfMail"];

                var rabbitMQueueName_UserConsumer = qPrefix + Configuration["AppConfig:RabbitMQueueName_UserConsumer"];

                var rabbitMQueueName_TmfRequest = qPrefix + Configuration["AppConfig:RabbitMQueueName_TmfRequest"];

                var rabbitMQueueName_TmfShareMail = qPrefix + Configuration["AppConfig:RabbitMQueueName_TmfShareMail"];

                var demoQueueName = qPrefix + Configuration["AppConfig:RabbitMQueueName_Demo"];

                var iwrsQueueName = qPrefix + Configuration["AppConfig:RabbitMQueueName_Iwrs"];
                var monitosingMailQueueName = qPrefix + Configuration["AppConfig:RabbitMQueueName_MonitoringMails"];

                var openQueryMailConsumerQueueName = qPrefix + Configuration["AppConfig:RabbitMQueueName_OpenQueryMailConsumer"];
                var visitMriFilesConsumerQueueName = qPrefix + Configuration["AppConfig:RabbitMQueueName_VisitMriFilesConsumer"];

                var doubleEntryStatusUpdateConsumer = qPrefix + Configuration["AppConfig:RabbitDoubleEntryStatusUpdateQueueName"];
                var addQueryMessagesConsumer = qPrefix + Configuration["AppConfig:RabbitAddQueryMessagesConsumerQueueName"];
                var addDoubleEntryQueryMessagesConsumer = qPrefix + Configuration["AppConfig:RabbitAddDoubleEntryQueryMessagesConsumerQueueName"];
                var addEProLinkMailConsumer = qPrefix + Configuration["AppConfig:RabbitAddEProLinkMailConsumerQueueName"];
                var MultiCycleAnnotatedPdfMailConsumer = qPrefix + Configuration["AppConfig:RabbitMultiCycleAnnotatedPdfMailConsumerQueueName"];

                var iwrsTransferMailConsumer = qPrefix + Configuration["AppConfig:RabbitIwrsMailConsumerQueueName"];

                var addEProCompletedMailConsumer = qPrefix + Configuration["AppConfig:RabbitMQueueName_EProCompletedMailQueueName"];
                var reportMailConsumer = qPrefix + Configuration["AppConfig:RabbitreportMailConsumerConsumerQueueName"];

                var settingMailConsumer = qPrefix + Configuration["AppConfig:RabbitMQueueName_SettingMailQueueName"];
                var relationInputConsumer = qPrefix + Configuration["AppConfig:RabbitMQueueName_RelationInputAuditQueueName"];


                //services.AddScoped<UserConsumer>();
                //services.AddScoped<DoubleEntryUserResearchStatusConsumer>();
                //services.AddScoped<ImportQueryMessagesConsumer>();
                //services.AddScoped<ImportDoubleEntryQueryMessagesConsumer>();
                //services.AddScoped<EProLinkMailConsumer>();
                //services.AddScoped<IwrsMailConsumer>();

                //services.AddScoped<TermOfUseSignedConsumer>();
                //services.AddScoped<AdversEventPdfMailConsumer>();
                //services.AddScoped<UserResearchMailConsumer>();
                //services.AddScoped<MultiCycleMailConsumer>();
                //services.AddScoped<CycleMailConsumer>();
                //services.AddScoped<ModuleDataStatusConsumerOverride>();
                //services.AddScoped<UserInputMailConsumer>();
                //services.AddScoped<TmfMailConsumer>();
                //services.AddScoped<TmfRequestConsumer>();
                //services.AddScoped<TmfShareConsumer>();
                //services.AddScoped<IwrsSiteAlertConsumer>();
                //services.AddScoped<MonitoringMailConsumer>();
                //services.AddScoped<OpenQueryMailConsumer>();
                //services.AddScoped<VisitMriFileConsumer>();
                //services.AddScoped<ArchivedMailConsumer>();
                //services.AddScoped<MultiCycleAnnotatedPdfMailConsumer>();
                //services.AddScoped<EProCompletedMailConsumer>();
                //services.AddScoped<SettingMailConsumer>();
                //services.AddScoped<ReportMailConsumer>();
                //services.AddScoped<RelationInputAuditConsumer>();

                //services.AddMassTransit(c =>
                //{
                //    c.AddConsumer<UserConsumer>();
                //    //c.AddConsumer<DoubleEntryUserResearchStatusConsumer>();
                //    //c.AddConsumer<ImportQueryMessagesConsumer>();
                //    //c.AddConsumer<ImportDoubleEntryQueryMessagesConsumer>();
                //    //c.AddConsumer<EProLinkMailConsumer>();
                //    //c.AddConsumer<TermOfUseSignedConsumer>();
                //    //c.AddConsumer<AdversEventPdfMailConsumer>();
                //    //c.AddConsumer<ModuleDataStatusConsumerOverride>();
                //    //c.AddConsumer<UserResearchMailConsumer>();
                //    //c.AddConsumer<MultiCycleMailConsumer>();
                //    //c.AddConsumer<CycleMailConsumer>();
                //    //c.AddConsumer<UserInputMailConsumer>();
                //    //c.AddConsumer<TmfMailConsumer>();
                //    //c.AddConsumer<TmfRequestConsumer>();
                //    //c.AddConsumer<TmfShareConsumer>();
                //    //c.AddConsumer<IwrsSiteAlertConsumer>();
                //    //c.AddConsumer<MonitoringMailConsumer>();
                //    //c.AddConsumer<OpenQueryMailConsumer>();
                //    //c.AddConsumer<VisitMriFileConsumer>();
                //    //c.AddConsumer<ArchivedMailConsumer>();
                //    //c.AddConsumer<MultiCycleAnnotatedPdfMailConsumer>();
                //    //c.AddConsumer<ReportMailConsumer>();
                //    //c.AddConsumer<IwrsMailConsumer>();
                //    //c.AddConsumer<EProCompletedMailConsumer>();
                //    //c.AddConsumer<SettingMailConsumer>();
                //    //c.AddConsumer<RelationInputAuditConsumer>();

                //    c.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                //    {
                //        cfg.Host(/*new Uri(rabbitMQUrl),*/ "rabbitmq://localhost",
                //              b => { b.Username(rabbitMQUserName); b.Password(rabbitMQPassword); });

                //        cfg.ReceiveEndpoint(rabbitMQueueName_UserConsumer, ep =>
                //        {
                //            ep.PrefetchCount = 16;
                //            ep.UseMessageRetry(r => r.Interval(2, 100));
                //            ep.ConfigureConsumer<UserConsumer>(provider);
                //        });
                //    }));
                //});

                //services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(
                //      cfg =>
                //      {
                //          cfg.Host(new Uri(rabbitMQUrl),
                //              b => { b.Username(rabbitMQUserName); b.Password(rabbitMQPassword); });

                //          //cfg.ReceiveEndpoint(rabbitMQueueName_UserConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    //e.Consumer<UserConsumer>(provider, p => { 

                //          //    //});
                //          //    EndpointConvention.Map<UserConsumer>(e.InputAddress);





                //          //});

                //          //cfg.ReceiveEndpoint(doubleEntryStatusUpdateConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<DoubleEntryUserResearchStatusConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<DoubleEntryUserResearchStatusConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(iwrsTransferMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<IwrsMailConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<IwrsMailConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(addQueryMessagesConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<ImportQueryMessagesConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<ImportQueryMessagesConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(addDoubleEntryQueryMessagesConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<ImportDoubleEntryQueryMessagesConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<ImportDoubleEntryQueryMessagesConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(addEProLinkMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<EProLinkMailConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<EProLinkMailConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(termofUseSignedConsumer, e =>
                //          //{

                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<TermOfUseSignedConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<TermOfUseSignedConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(visitMriFilesConsumerQueueName, e =>
                //          //{

                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<VisitMriFileConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<VisitMriFileConsumer>(e.InputAddress);
                //          //});


                //          //cfg.ReceiveEndpoint(openQueryMailConsumerQueueName, e =>
                //          //{

                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<OpenQueryMailConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<OpenQueryMailConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(monitosingMailQueueName, e =>
                //          //{

                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<MonitoringMailConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<MonitoringMailConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(iwrsQueueName, e =>
                //          //{

                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<IwrsSiteAlertConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<IwrsSiteAlertConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(mailQueueUserInputMailConsumer, e =>
                //          //{

                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<UserInputMailConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<UserInputMailConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(rabbitMQueueName_TmfMail, e =>
                //          //{

                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<TmfMailConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<TmfMailConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(rabbitMQueueName_TmfRequest, e =>
                //          //{

                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<TmfRequestConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<TmfRequestConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(rabbitMQueueName_TmfShareMail, e =>
                //          //{

                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<TmfShareConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<TmfShareConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(mailQueueCycleMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<CycleMailConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<CycleMailConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(mailQueueMultiCycleMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<MultiCycleMailConsumer>(provider, p =>
                //          //    {

                //          //    });
                //          //    EndpointConvention.Map<MultiCycleMailConsumer>(e.InputAddress);
                //          //});

                //          //cfg.ReceiveEndpoint(mailQueueUserResearchMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<UserResearchMailConsumer>(provider, p =>
                //          //    {
                //          //    });
                //          //    EndpointConvention.Map<UserResearchMailConsumer>(e.InputAddress);
                //          //});
                //          //cfg.ReceiveEndpoint(mailQueueNameAdversEventPdfMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<AdversEventPdfMailConsumer>(provider, p =>
                //          //    {
                //          //    });
                //          //    EndpointConvention.Map<AdversEventPdfMailConsumer>(e.InputAddress);
                //          //});
                //          //cfg.ReceiveEndpoint(moduleDataStatusQueueName, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<ModuleDataStatusConsumerOverride>(provider, p =>
                //          //    {
                //          //    });
                //          //    EndpointConvention.Map<ModuleDataStatusConsumerOverride>(e.InputAddress);
                //          //});
                //          //cfg.ReceiveEndpoint(mailQueueArchivedMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<ArchivedMailConsumer>(provider, p =>
                //          //    {
                //          //    });
                //          //    EndpointConvention.Map<ArchivedMailConsumer>(e.InputAddress);
                //          //});
                //          //cfg.ReceiveEndpoint(MultiCycleAnnotatedPdfMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<MultiCycleAnnotatedPdfMailConsumer>(provider, p =>
                //          //    {
                //          //    });
                //          //    EndpointConvention.Map<MultiCycleAnnotatedPdfMailConsumer>(e.InputAddress);
                //          //});
                //          //cfg.ReceiveEndpoint(addEProCompletedMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<EProCompletedMailConsumer>(provider, p =>
                //          //    {
                //          //    });
                //          //    EndpointConvention.Map<EProCompletedMailConsumer>(e.InputAddress);

                //          //});
                //          //cfg.ReceiveEndpoint(settingMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<SettingMailConsumer>(provider, p =>
                //          //    {
                //          //    });
                //          //    EndpointConvention.Map<SettingMailConsumer>(e.InputAddress);
                //          //});
                //          //cfg.ReceiveEndpoint(reportMailConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<ReportMailConsumer>(provider, p =>
                //          //    {
                //          //    });
                //          //    EndpointConvention.Map<ReportMailConsumer>(e.InputAddress);
                //          //});
                //          //cfg.ReceiveEndpoint(relationInputConsumer, e =>
                //          //{
                //          //    e.PrefetchCount = 16;
                //          //    e.UseMessageRetry(x => x.Interval(2, 5000));
                //          //    e.Consumer<RelationInputAuditConsumer>(provider, p =>
                //          //    {
                //          //    });
                //          //    EndpointConvention.Map<RelationInputAuditConsumer>(e.InputAddress);
                //          //});
                //      }));
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
                //services.AddSingleton<IHostedService, Helios.EventBus.Base.MassTransitBusHostedService>();
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
        }
    }
}
