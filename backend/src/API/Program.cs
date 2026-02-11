using DotNetEnv;
using Infrastructure;
// using Application;
using Microsoft.EntityFrameworkCore;
//cargamos dotenv
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost"};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT") ?? "5432"};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration, connectionString);
// builder.Services.AddApplication();

//Servicios de la API
builder.Services.AddControllers();
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
