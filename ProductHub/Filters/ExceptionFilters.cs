using System.Net;
using Apps_ADM.Application.Common.BaseResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProductHub.Application.Common.Exceptions;

namespace ProductHub.Filters;

public class ExceptionFilters : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<ExceptionFilters>>();

        context.Result = context.Exception switch
        {
            NotFoundException notFoundException => HandleNotFoundException(notFoundException),
            BadRequestException badRequestException => HandleBadRequestException(badRequestException),
            _ => HandleUnhandledException(context.Exception, logger)
        };

        context.ExceptionHandled = true;
    }

    private static IActionResult HandleNotFoundException(NotFoundException notFoundException)
    {
        return new ObjectResult(notFoundException.Response)
        {
            StatusCode = (int)notFoundException.Response.StatusCode
        };
    }

    private static IActionResult HandleBadRequestException(BadRequestException badRequestException)
    {
        return new ObjectResult(badRequestException.Response)
        {
            StatusCode = (int)badRequestException.Response.StatusCode
        };
    }

    private static IActionResult HandleUnhandledException(Exception exception, ILogger logger)
    {
        var errorResponse = BaseResponse.InternalServerError(exception.Message);
        logger.LogError(exception, "An unhandled exception has occurred at {Time}: {Message}", DateTime.Now, exception.Message);

        return new ObjectResult(errorResponse)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
}
