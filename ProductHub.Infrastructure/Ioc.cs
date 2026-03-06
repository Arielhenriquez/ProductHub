using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductHub.Application.Interfaces;
using ProductHub.Infrastructure.Persistence.Context;
using ProductHub.Infrastructure.Repositories;

namespace ProductHub.Infrastructure;

public static class Ioc
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<ProductHubContext>(options => options.UseSqlServer(connectionString));

        services.AddTransient<IDbContext, ProductHubContext>();
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IBlobService, BlobService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        // Seeds a default Admin on first startup
        services.AddHostedService<AdminSeederService>();

        return services;
    }
}

