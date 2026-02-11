using Application.Interfaces;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        // Aquí agregaremos IAuthService más adelante
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}