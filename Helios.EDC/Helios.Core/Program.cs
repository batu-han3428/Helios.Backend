using Helios.Core.Contexts;
using Helios.Core.Extension;
using Helios.Core.Helpers;
using Helios.Caching.Services.Interfaces;
using Helios.Caching.Services;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = configuration.GetConnectionString("Redis");
    options.InstanceName = "Helios:";
});

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

// Add services to the container.

var activeSql = builder.Configuration["AppConfig:ActiveSql"];
builder.Services.ConfigureMysqlPooled<CoreContext>(builder.Configuration, connectionString: $"{activeSql}_DefaultConnection", MigrationsAssembly: "Helios.Core");

#region Smtp
builder.Services.SmtpSettings();
#endregion

#region Dependency Injection 
builder.Services.DependencyInjection();
#endregion

builder.Services.AddControllers();

// Add Redis Cache Service
//builder.Services.AddRedisCache(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddMemoryCache();
builder.Services.DefaultConfigurationService(configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();
//builder.Services.DefaultConfigurationService(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        // Swagger UI'�n k�k dizinine y�nlendirme
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
