
using FluentValidation;
using MinimalAPIPeliculas.DTOs;

namespace MinimalAPIPeliculas.Filtros
{
    public class FiltroValidaciones<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator is null)
            {
                return await next(context);
            }

            var insumoAValidar = context.Arguments.OfType<T>().FirstOrDefault();

            if (insumoAValidar is null)
            {
                return TypedResults.Problem("No pudo ser encontrada la propiedad a validar");
            }

            var resultadoAValidacion = await validator.ValidateAsync(insumoAValidar);

            if (!resultadoAValidacion.IsValid)
            {
                return TypedResults.ValidationProblem(resultadoAValidacion.ToDictionary());
            }

            return await next(context);
        }
    }
}
