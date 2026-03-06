using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductHub.Application.Interfaces;
using ProductHub.Domain.Models;

namespace ProductHub.Infrastructure;

/// <summary>
/// Runs once at startup. Creates a default Admin user if none exists.
/// Configure via appsettings.json → AdminSeed (or env vars AdminSeed__Email, etc.)
/// </summary>
public class AdminSeederService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminSeederService> _logger;

    public AdminSeederService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<AdminSeederService> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var email = _configuration["AdminSeed:Email"];
        var password = _configuration["AdminSeed:Password"];
        var name = _configuration["AdminSeed:Name"] ?? "Admin";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("AdminSeed:Email or AdminSeed:Password not configured. Skipping admin seeder.");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var userRepo = scope.ServiceProvider.GetRequiredService<IBaseRepository<Users>>();

        // Idempotent: skip if any active admin already exists
        var adminExists = await userRepo.Query()
            .AnyAsync(u => u.Role == "Admin" && !u.IsDeleted, cancellationToken);

        if (adminExists)
        {
            _logger.LogInformation("Admin user already exists. Skipping seeder.");
            return;
        }

        var nameParts = name.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var admin = new Users
        {
            FirstName = nameParts[0],
            LastName = nameParts.Length > 1 ? nameParts[1] : null,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = "Admin",
            IsActive = true
        };

        await userRepo.AddAsync(admin, cancellationToken);
        _logger.LogInformation("Default admin user '{Email}' created successfully.", email);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
