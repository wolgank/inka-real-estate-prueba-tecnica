using System.Net;

namespace API.Exceptions;

public interface IExceptionHandler
{
    bool CanHandle(Exception exception);
    (HttpStatusCode Status, object Body) Handle(Exception exception);
}