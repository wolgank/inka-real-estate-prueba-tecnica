using System.Net;

namespace API.Exceptions;

public class ForbiddenExceptionHandler : IExceptionHandler
{
    // En .NET, el acceso denegado por roles a veces no lanza una excepción atrapable 
    // por middleware de la misma forma, pero si usas políticas personalizadas sí.
    // Por ahora, manejemos accesos denegados generales:
    public bool CanHandle(Exception exception) => exception is UnauthorizedAccessException; 

    public (HttpStatusCode Status, object Body) Handle(Exception exception)
    {
        return (HttpStatusCode.Forbidden, new { Message = "No tienes permisos suficientes para realizar esta acción." });
    }
}