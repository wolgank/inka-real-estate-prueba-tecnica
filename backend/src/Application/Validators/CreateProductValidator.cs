using Application.DTOs.Products;
using FluentValidation;

namespace Application.Validators;

public class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("El precio debe ser mayor a cero.");
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");
        RuleFor(x => x.Category).NotEmpty();
    }
}