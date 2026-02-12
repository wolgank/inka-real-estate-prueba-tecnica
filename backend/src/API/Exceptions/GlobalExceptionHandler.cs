using System.Net;

namespace API.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    // Este maneja CUALQUIER excepción (siempre devuelve true como última opción)
    public bool CanHandle(Exception exception) => true;

    public (HttpStatusCode Status, object Body) Handle(Exception exception)
    {
        return (HttpStatusCode.InternalServerError, new 
        { 
            Message = "Ocurrió un error inesperado en el servidor. Por favor, contacte al soporte técnico.",
            // En producción, nunca envíes exception.Message aquí por seguridad
        });
    }
}