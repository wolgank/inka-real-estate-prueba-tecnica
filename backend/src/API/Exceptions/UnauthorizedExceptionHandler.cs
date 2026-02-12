using System.Net;

namespace API.Exceptions;

public class UnauthorizedExceptionHandler : IExceptionHandler
{
    // Este manejador responde ante UnauthorizedAccessException
    public bool CanHandle(Exception exception) => exception is UnauthorizedAccessException;

    public (HttpStatusCode Status, object Body) Handle(Exception exception)
    {
        return (HttpStatusCode.Unauthorized, new 
        { 
            Message = "No tiene permisos para acceder a este recurso o sus credenciales son inválidas.",
            Type = "Unauthorized"
        });
    }
}