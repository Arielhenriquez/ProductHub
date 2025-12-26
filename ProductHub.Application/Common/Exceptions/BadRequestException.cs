using Response = ProductHub.Application.Common.BaseResponse.BaseResponse;
namespace ProductHub.Application.Common.Exceptions;


public class BadRequestException(string message) : Exception(message)
{
    public Response Response { get; } = Response.BadRequest(message);
}
