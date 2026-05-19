using AdoptMeApi.Modelo;
using FluentValidation;

namespace AdoptMeApi.Validaciones
{
    public class PatchedMascotaValidator : AbstractValidator<PatchedMascota>
    {
        public PatchedMascotaValidator()
        {
            // 1. Validar el Nombre SOLO si se ha enviado en el JSON
            RuleFor(m => m.Nombre)
                .NotEmpty().WithMessage("Si envías el nombre, este no puede estar vacío.")
                .MaximumLength(50).WithMessage("El nombre no puede tener más de 50 caracteres.")
                .When(m => m.Nombre != null); // <-- La regla mágica para PATCH

            // 2. Validar la Especie SOLO si se ha enviado
            RuleFor(m => m.Especie)
                .NotEmpty().WithMessage("Si envías la especie, esta no puede estar vacía.")
                .When(m => m.Especie != null);

            // 3. Validar la Edad SOLO si se ha enviado
            RuleFor(m => m.Edad)
                .GreaterThanOrEqualTo(0).WithMessage("La edad no puede ser un número negativo.")
                .LessThanOrEqualTo(30).WithMessage("Por favor, introduce una edad realista (menor a 30 años).")
                .When(m => m.Edad.HasValue); // Para tipos int?, usamos .HasValue

            // 4. Validar el Estado de Adopción SOLO si se ha enviado
            RuleFor(m => m.EstadoAdopcion)
                .Must(estado => estado == "Disponible" || estado == "Adoptado")
                .WithMessage("El estado de adopción debe ser únicamente 'Disponible' o 'Adoptado'.")
                .When(m => m.EstadoAdopcion != null);
        }
    }
}
