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
        // Services

        services.AddScoped<ICategoryService, CategoryService>();
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }

}
