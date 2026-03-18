using FluentValidation;
using SistemaReservaGimnasioBK.Application.DTOs;

namespace SistemaReservaGimnasioBK.Application.Validators;

/// <summary>
/// Validador para CreateSpaceDto
/// </summary>
public class CreateSpaceDtoValidator : AbstractValidator<CreateSpaceDto>
{
    public CreateSpaceDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre del espacio es requerido")
            .MaximumLength(100)
            .WithMessage("El nombre no puede exceder 100 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("La descripción no puede exceder 500 caracteres");

        RuleFor(x => x.Capacity)
            .GreaterThan(0)
            .WithMessage("La capacidad debe ser mayor a cero")
            .LessThanOrEqualTo(100)
            .WithMessage("La capacidad no puede exceder 100 personas");

        RuleFor(x => x.Type)
            .Must(type => Enum.IsDefined(typeof(SistemaReservaGimnasioBK.Domain.ValueObjects.SpaceType), type))
            .WithMessage("El tipo de espacio no es válido");
    }
}
