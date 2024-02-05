using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories
{
    public interface IRepositorioActores
    {
        Task Actualizar(Actor actor);
        Task<int> Crear(Actor actor);
        Task Eliminar(int id);
        Task<bool> Existe(int id);
        Task<Actor?> ObtenerPorId(int id);
        Task<List<Actor>> ObtenerTodos();
    }
}