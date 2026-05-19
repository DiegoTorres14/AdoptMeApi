using FluentValidation;
using AdoptMeApi.Modelo;

namespace AdoptMeApi.Validaciones
{
    public class MascotaValidator : AbstractValidator<Mascota>
    {
        public MascotaValidator()
        {
            // 1. El nombre es obligatorio y no puede ser solo espacios
            RuleFor(m => m.Nombre)
                .NotEmpty().WithMessage("El nombre de la mascota es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede tener más de 50 caracteres.");

            // 2. La especie es obligatoria
            RuleFor(m => m.Especie)
                .NotEmpty().WithMessage("La especie (Perro, Gato, etc.) es obligatoria.");

            // 3. La edad no puede ser negativa ni absurdamente alta
            RuleFor(m => m.Edad)
                .GreaterThanOrEqualTo(0).WithMessage("La edad no puede ser un número negativo.")
                .LessThanOrEqualTo(30).WithMessage("Por favor, introduce una edad realista (menor a 30 años).");

            // 4. El estado de adopción solo puede ser uno de los permitidos
            RuleFor(m => m.EstadoAdopcion)
                .Must(estado => estado == "Disponible" || estado == "Adoptado")
                .WithMessage("El estado de adopción debe ser únicamente 'Disponible' o 'Adoptado'.");
        }
    }
}
