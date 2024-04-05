using MinimalAPIPeliculas.Data;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Models;

namespace MinimalAPIPeliculas.GraphQL
{
    public class Query
    {
        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Genero> ObtenerGeneros([Service] ApplicationDbContext context) => context.Generos;

        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Pelicula> ObtenerPeliculas([Service] ApplicationDbContext context) => context.Peliculas;

        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Actor> ObtenerActor([Service] ApplicationDbContext context) => context.Actores;
    }
}
