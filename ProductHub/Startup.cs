using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductHub.Application;
using ProductHub.Domain.Settings;
using ProductHub.Filters;
using ProductHub.Infrastructure;
using ProductHub.Middlewares;
using System.Text.Json.Serialization;

namespace ProductHub;

public class Startup
{
    public Startup(ConfigurationManager configuration)
    {
        Configuration = configuration;
    }
    public ConfigurationManager Configuration { get; set; }

    public void RegisterServices(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add<ExceptionFilters>();
        }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        // ──────────────────────────────────────────
        // JWT Settings
        // ──────────────────────────────────────────
        var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings is not configured.");

        services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));
        services.Configure<StorageOptions>(Configuration.GetSection("BlobStorage"));

        // ──────────────────────────────────────────
        // JWT Authentication
        // ──────────────────────────────────────────
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    await UnauthorizedHelper.HandleUnauthorizedResponse(
                        context,
                        "Tu token es invalido o expiro. Debes logearte de nuevo."
                    );
                }
            };
        });

        services.AddAuthorization();

        services.AddHttpContextAccessor();
        services.AddInfrastructure(Configuration);
        services.AddApplication();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductHub.Api", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. Example: Bearer eyJhbGci..."
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        services.AddCors(options =>
        {
            options.AddPolicy("DevPolicy", builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
                builder.SetIsOriginAllowed(x => true);
            });
        });
    }

    public void SetupMiddlewares(WebApplication app)
    {
        app.UseCors("DevPolicy");
        app.UseMiddleware<ForbiddenMiddleware>();
    }
}
