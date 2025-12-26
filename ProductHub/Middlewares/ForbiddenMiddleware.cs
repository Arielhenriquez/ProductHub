using System.Net;
using Apps_ADM.Application.Common.BaseResponse;
using Newtonsoft.Json;

namespace ProductHub.Middlewares;

public class ForbiddenMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
        {
            var errorMessage = "No tienes permiso para usar este endpoint";
            var forbiddenResponse = BaseResponse.Forbidden(errorMessage);

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(forbiddenResponse));
        }
    }
}
