
using System.Net;
using API.Exceptions;

public class ValidationExceptionHandler : IExceptionHandler
{
    public bool CanHandle(Exception exception) => exception is FluentValidation.ValidationException;

    public (HttpStatusCode Status, object Body) Handle(Exception exception)
    {
        var ve = (FluentValidation.ValidationException)exception;
        return (HttpStatusCode.BadRequest, new {
            Message = "Error de validación",
            Errors = ve.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
        });
    }
}