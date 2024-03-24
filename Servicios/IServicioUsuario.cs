using Microsoft.AspNetCore.Identity;

namespace MinimalAPIPeliculas.Servicios
{
    public interface IServicioUsuario
    {
        Task<IdentityUser?> ObtenerUsuario();
    }
}