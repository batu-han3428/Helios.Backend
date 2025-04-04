using Helios.Core.Contexts;
using Helios.Core.Extension;
using Helios.Core.Helpers;
using Helios.Caching.Services.Interfaces;
using Helios.Caching.Services;
using Helios.Caching.Helpers;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration; // allows both to access and to set up the config



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
builder.Services.AddRedisCache(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.DefaultConfigurationService(configuration);

builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen();
//builder.Services.DefaultConfigurationService(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("MyPolicy");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
