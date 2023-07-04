using Microsoft.EntityFrameworkCore;

namespace Helios.Authentication.Helpers
{
    public static class ConfigureDb
    {
        public static void ConfigureMysqlPooled<TContext>(this IServiceCollection services, IConfiguration configuration,
            string connectionString = "DefaultConnection", string MigrationsAssembly = "", int commandTimeout = 120) where TContext : DbContext
        {
            var conApplication = configuration.GetConnectionString(connectionString);
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 15));

            services.AddDbContextPool<TContext>(
                  (services, options) =>
                  {
                      options.UseMySql(conApplication, serverVersion, mySqlOptions =>
                      {
                          mySqlOptions.MigrationsAssembly(MigrationsAssembly);
                          mySqlOptions.CommandTimeout(commandTimeout);
                      }).EnableSensitiveDataLogging().EnableDetailedErrors();
                  });
        }

        public static void ConfigureMysql<TContext>(this IServiceCollection services, IConfiguration configuration,
           string connectionString = "DefaultConnection", string MigrationsAssembly = "", int commandTimeout = 120) where TContext : DbContext
        {
            var conApplication = configuration.GetConnectionString(connectionString);
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));
            services.AddDbContext<TContext>(
                   options => options.UseMySql(conApplication, serverVersion,
                       mySqlOptions =>
                       {
                           mySqlOptions.MigrationsAssembly(MigrationsAssembly);
                           mySqlOptions.CommandTimeout(commandTimeout);
                       }
                  ).EnableSensitiveDataLogging().EnableDetailedErrors());
        }
    }
    public class ServiceInstallerHelper
    {



    }
}
