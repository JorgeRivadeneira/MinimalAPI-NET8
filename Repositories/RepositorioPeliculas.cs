using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Data;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Utilities;

namespace MinimalAPIPeliculas.Repositories
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly HttpContext httpContext;

        public RepositorioPeliculas(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<List<Pelicula>> ObtenerTodos(PaginacionDTO paginacionDTO)
        {
            var queryable = context.Peliculas.AsQueryable();
            await httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(x => x.Titulo).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<Pelicula> ObtenerPorId(int id)
        {
            return await context.Peliculas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> Crear(Pelicula pelicula)
        {
            context.Add(pelicula);
            await context.SaveChangesAsync();
            return pelicula.Id;
        }

        public async Task Actualizar(Pelicula pelicula)
        {
            context.Update(pelicula);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Peliculas.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Peliculas.AnyAsync(x => x.Id == id);
        }
    }
}
