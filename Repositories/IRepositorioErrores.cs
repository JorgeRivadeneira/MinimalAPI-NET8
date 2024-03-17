using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories
{
    public interface IRepositorioErrores
    {
        Task Crear(Error error);
    }
}