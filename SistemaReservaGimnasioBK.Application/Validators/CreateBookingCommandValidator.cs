using FluentValidation;

namespace SistemaReservaGimnasioBK.Application.Validators;

/// <summary>
/// Validador para el comando CreateBookingCommand usando FluentValidation.
/// En DDD, las validaciones en la capa Application son diferentes a las del Domain:
/// - Application Layer: Valida formato, requisitos técnicos, reglas simples
/// - Domain Layer: Valida invariantes de negocio complejas
/// 
/// FluentValidation proporciona una forma declarativa y fluida de definir reglas de validación.
/// </summary>
public class CreateBookingCommandValidator : AbstractValidator<Commands.CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        // Validar que SpaceId no sea vacío
        RuleFor(x => x.SpaceId)
            .NotEmpty()
            .WithMessage("El ID del espacio es requerido");

        // Validar que UserId no sea vacío
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("El ID del usuario es requerido");

        // Validar que StartDateTime sea futuro
        RuleFor(x => x.StartDateTime)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("La fecha de inicio debe ser en el futuro");

        // Validar que EndDateTime sea posterior a StartDateTime
        RuleFor(x => x.EndDateTime)
            .GreaterThan(x => x.StartDateTime)
            .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio");

        // Validar que la duración no sea excesiva (máximo 8 horas por ejemplo)
        RuleFor(x => x)
            .Must(x => (x.EndDateTime - x.StartDateTime).TotalHours <= 8)
            .WithMessage("La duración máxima de una reserva es de 8 horas");

        // Validar que la duración no sea muy corta (mínimo 30 minutos)
        RuleFor(x => x)
            .Must(x => (x.EndDateTime - x.StartDateTime).TotalMinutes >= 30)
            .WithMessage("La duración mínima de una reserva es de 30 minutos");

        // Validar longitud de notas (opcional)
        When(x => !string.IsNullOrEmpty(x.Notes), () =>
        {
            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .WithMessage("Las notas no pueden exceder 500 caracteres");
        });
    }
}
