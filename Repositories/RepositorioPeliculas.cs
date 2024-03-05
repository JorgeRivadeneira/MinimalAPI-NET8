using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Data;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Endpoints;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Utilities;

namespace MinimalAPIPeliculas.Repositories
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public RepositorioPeliculas(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
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
            return await context.Peliculas
                    .Include(p => p.Comentarios)
                    .Include(p => p.GenerosPelicula)
                        .ThenInclude(gp => gp.Genero)
                    .Include(p => p.ActoresPelicula.OrderBy(x => x.Orden))
                        .ThenInclude(ap => ap.Actor)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);
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

        public async Task AsignarGeneros(int id, List<int> generosIds)
        {
            var pelicula = await context.Peliculas
                .Include(p => p.GenerosPelicula)
                .FirstOrDefaultAsync(p => p.Id == id);

            if(pelicula is null)
            {
                throw new ArgumentException($"No Existe película con el id: {id}");
            }

            var generosPelicula = generosIds.Select(generosId => new GeneroPelicula() { GeneroId = generosId });

            pelicula.GenerosPelicula = mapper.Map(generosPelicula, pelicula.GenerosPelicula);

            await context.SaveChangesAsync();
        }

        public async Task AsignarActores(int peliculaId, List<ActorPelicula> actores)
        {
            for(int i = 1; i < actores.Count; i++)
            {
                actores[i - 1].Orden = i;
            }

            var pelicula = await context.Peliculas
                    .Include(p => p.ActoresPelicula)
                    .FirstOrDefaultAsync(p => p.Id == peliculaId);
            if( pelicula is null)
            {
                throw new ArgumentException($"No existe la pelicula con el id: {peliculaId}");
            }

            pelicula.ActoresPelicula = mapper.Map(actores, pelicula.ActoresPelicula);
            await context.SaveChangesAsync();
        }
    }
}
