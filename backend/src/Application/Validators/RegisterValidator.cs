using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(4);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Formato de correo inválido.");
        RuleFor(x => x.Dni)
            .NotEmpty()
            .Length(8)
            .Matches("^[0-9]*$").WithMessage("El DNI debe contener solo 8 dígitos.");
        RuleFor(x => x.PhoneNumber).NotEmpty();
    }
}