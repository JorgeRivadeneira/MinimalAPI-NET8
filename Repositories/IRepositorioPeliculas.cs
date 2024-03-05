using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Endpoints;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories
{
    public interface IRepositorioPeliculas
    {
        Task Actualizar(Pelicula pelicula);
        Task AsignarActores(int peliculaId, List<ActorPelicula> actores);
        Task AsignarGeneros(int id, List<int> generosIds);
        Task Borrar(int id);
        Task<int> Crear(Pelicula pelicula);
        Task<bool> Existe(int id);
        Task<Pelicula> ObtenerPorId(int id);
        Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO);
    }
}