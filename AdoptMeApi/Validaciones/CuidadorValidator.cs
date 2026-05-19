using AdoptMeApi.Modelo;
using FluentValidation;

namespace AdoptMeApi.Validaciones
{
    public class CuidadorValidator : AbstractValidator<Cuidador>
    {
        public CuidadorValidator() 
        {
            // 1. Nombre obligatorio para el Cuidador y no exceder de los 50 caracteres
            RuleFor(c => c.Nombre)
                .NotEmpty().WithMessage("El nombre del cuidador no puede estar vacío")
                .MaximumLength(50).WithMessage("La longitud máxima del nombre no puede ser superior a 50 caracteres");

            // 2. Teléfono
            RuleFor(c => c.Telefono)
                .NotEmpty().WithMessage("El teléfono del cuidador es obligatorio.")
                .Length(9).WithMessage("El teléfono debe tener exactamente 9 dígitos.")
                .Matches(@"^[0-9]+$").WithMessage("El teléfono solo puede contener números.");
        }
    }
}
