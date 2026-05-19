using AdoptMeApi.Modelo;
using FluentValidation;

namespace AdoptMeApi.Validaciones
{
    public class PatchedCuidadorValidator : AbstractValidator<PatchedCuidador>
    {
        public PatchedCuidadorValidator() 
        { 
            // 1. Validar nombre
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre del cuidador no puede estar vacío")
                .MaximumLength(50).WithMessage("La longitud máxima del nombre no puede ser superior a 50 caracteres")
                .When(m => m.Nombre != null); // <- Cuando no esté nulo en el json se aplicará la norma

            // 2. Validar teléfono
            RuleFor(c => c.Telefono)
                .NotEmpty().WithMessage("El teléfono del cuidador es obligatorio.")
                .Length(9).WithMessage("El teléfono debe tener exactamente 9 dígitos.")
                .Matches(@"^[0-9]+$").WithMessage("El teléfono solo puede contener números.")
                .When(c => c.Telefono != null);
        }
    }
}
