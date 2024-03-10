using FluentValidation;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Repositories;

namespace MinimalAPIPeliculas.Validaciones
{
    public class CrearGeneroDTOValidador:AbstractValidator<CrearGeneroDTO>
    {
        public CrearGeneroDTOValidador(IRepositorioGeneros repositorioGeneros, IHttpContextAccessor httpContextAccessor)
        {
            var valorDeRutaId = httpContextAccessor.HttpContext?.Request.RouteValues["id"]; 
            var id = 0;

            //TODO: Ver lógica de conversiones para aplicar
            if (valorDeRutaId is string valorString)
            {
                int.TryParse(valorString, out id);
            }

            //Validaciones FluentValidation
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("El campo {PropertyName} es obligatorio")
                .MaximumLength(50).WithMessage("El campo {PropertyName} debe tener menos de {MaxLength} caracteres")
                .Must(ValidarPrimeraLetraMayuscula).WithMessage("El campo {PropertyName} debe comenzar con mayúsculas")
                .MustAsync(async (nombre, _) =>
                {
                    var existe = await repositorioGeneros.Existe(id, nombre);
                    return !existe;
                }).WithMessage(g => $"Ya existe un género con el nombre {g.Nombre}");
        }

        private bool ValidarPrimeraLetraMayuscula(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }

            var primeraLetra = valor[0].ToString();
            return primeraLetra == primeraLetra.ToUpper();
        }
    }
}
