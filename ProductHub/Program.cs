using Microsoft.AspNetCore.HttpLogging;
using ProductHub;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHttpLogging(httpLogging =>
{
    httpLogging.LoggingFields = HttpLoggingFields.All;
});
var setup = new Startup(builder.Configuration);

setup.RegisterServices(builder.Services);

var app = builder.Build();

setup.SetupMiddlewares(app);

app.UseSwagger();
app.UseSwaggerUI();

// Redirect root "/" to Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();