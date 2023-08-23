using Helios.Authentication.Contexts;
using Helios.Authentication.Extension;
using Helios.Authentication.Helpers;
using System.Configuration;
using System.Net.Mail;
using System.Net;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Microsoft.Extensions.Configuration.ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config
    IWebHostEnvironment env = builder.Environment;


    #region Identity
        builder.Services.IdentityServerSettings(configuration);
        builder.Services.CookieSettings();
    #endregion

    #region Rabbit Mq Pub/Sub
        builder.Services.ConfigurationEventBusConsumers(configuration, env);
        builder.Services.ConfigurationEventBusJob();
    #endregion

    #region Smtp
        builder.Services.SmtpSettings();
    #endregion

    #region Dependency Injection 
        builder.Services.DependencyInjection();
    #endregion


    //builder.Services.AddDbContext<AuthenticationContext>(options =>
    //{
    //    options.UseMySQL(connectionString: $"{activeSql}_DefaultConnection");
    //});

    //builder.Services.AddMySQLServer<AuthenticationContext>(connectionString: $"{activeSql}_DefaultConnection");
    //builder.Services.AddDbContext<AuthenticationContext>(builder.Configuration, connectionString: $"{activeSql}_DefaultConnection", MigrationsAssembly: "auth");

    //builder.Services.AddDbContext<AuthenticationContext>(builder.Configuration, connectionString: $"{activeSql}_DefaultConnection", MigrationsAssembly: "Helios.Authentication");

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception e)
{

	throw;
}
