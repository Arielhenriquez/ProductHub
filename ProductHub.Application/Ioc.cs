using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ProductHub.Application.Interfaces;
using ProductHub.Application.Services;

namespace ProductHub.Application;

public static class Ioc
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductImageService, ProductImageService>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
}
