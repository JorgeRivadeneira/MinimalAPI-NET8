using FluentValidation;
using MinimalAPIPeliculas.DTOs;

namespace MinimalAPIPeliculas.Validaciones
{
    public class CrearActorDTOValidador : AbstractValidator<CrearActorDTO>
    {
        public CrearActorDTOValidador()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("El campo {PropertyName} es requerido")
                .MaximumLength(150).WithMessage("El campo {PropertyName} debe tener menos de {MaxLength} caracteres");
        }
    }
}
