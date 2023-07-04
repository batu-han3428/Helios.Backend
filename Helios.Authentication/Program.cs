using Helios.Authentication.Contexts;
using Helios.Authentication.Helpers;
using MySql.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var activeSql = builder.Configuration["AppConfig:ActiveSql"];
builder.Services.ConfigureMysqlPooled<AuthenticationContext>(builder.Configuration, connectionString: $"{activeSql}_DefaultConnection", MigrationsAssembly: "Helios.Authentication");

builder.Services.AddMySQLServer<AuthenticationContext>(connectionString: $"{activeSql}_DefaultConnection");
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
