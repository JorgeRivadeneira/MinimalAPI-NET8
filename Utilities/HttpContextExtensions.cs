using Microsoft.EntityFrameworkCore;

namespace MinimalAPIPeliculas.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task InsertarParametrosPaginacionEnCabecera<T>(this HttpContext context, IQueryable<T> queryable)
        {
            if(context == null) throw new ArgumentNullException(nameof(context));
            double cantidad = await queryable.CountAsync();
            context.Response.Headers.Append("cantidadTotalRegistros", cantidad.ToString());
        }
    }
}
