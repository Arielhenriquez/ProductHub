using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.OpenApi.Models;
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

      //  services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      //.AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAd"));

        services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
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

        services.AddHttpContextAccessor();
        services.AddInfrastructure(Configuration);
        //services.AddApplication();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProductHub.Api", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                In = ParameterLocation.Header
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
            options.AddPolicy("DevPolicy",
                builder =>
                {
                    builder
                        .AllowAnyHeader()
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
