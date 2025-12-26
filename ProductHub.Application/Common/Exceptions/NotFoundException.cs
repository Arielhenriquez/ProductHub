using Response = ProductHub.Application.Common.BaseResponse.BaseResponse;
namespace ProductHub.Application.Common.Exceptions;


public class NotFoundException : Exception
{
    public Response Response { get; }

    public NotFoundException(string message)
      : base(message)
    {
        Response = Response.NotFound(message);
    }

    public NotFoundException(string entityName, Guid id)
      : base($"{entityName} con id: {id} no encontrado")
    {
        Response = Response.NotFound(Message);
    }
}
