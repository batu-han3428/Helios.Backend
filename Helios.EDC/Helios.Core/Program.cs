using Helios.Core.Contexts;
using Helios.Core.Extension;
using Helios.Core.Helpers;

var builder = WebApplication.CreateBuilder(args);

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        // Swagger UI'ýn kök dizinine yönlendirme
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
