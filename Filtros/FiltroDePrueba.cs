using AutoMapper;
using MinimalAPIPeliculas.Repositories;
using System;

public class FiltroDePrueba: IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        //se ejecuta antes del endpoint
        var paramRepositorioGeneros = context.Arguments.OfType<IRepositorioGeneros>().FirstOrDefault();
        var paramEntero = context.Arguments.OfType<int>().FirstOrDefault();
        var param3 = context.Arguments.OfType<IMapper>().FirstOrDefault();

        var resultado = await next(context);

        //se ejecuta después del endpoint
        return resultado;
    }

}
