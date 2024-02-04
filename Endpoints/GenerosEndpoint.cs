using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Models;
using MinimalAPIPeliculas.Repositories;

namespace MinimalAPIPeliculas.Endpoints
{
    public static class GenerosEndpoint
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));
            group.MapGet("/{id:int}", ObtenerGeneroPorId);
            group.MapPost("/", CrearGenero);
            group.MapPut("/{id:int}", ActualizarGenero);
            group.MapDelete("/", EliminarGenero);

            return group;
        }

        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper)
        {
            var generos = await repositorio.ObtenerTodos();
            //var generosDTO = generos.Select(x => new GeneroDTO { Id = x.Id, Nombre = x.Nombre }).ToList();
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);  //List<Genero> => List<GeneroDTO>
            return TypedResults.Ok(generosDTO);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(IRepositorioGeneros repositorio, int id, IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return TypedResults.Ok(generoDTO);
        }

        static async Task<Created<GeneroDTO>> CrearGenero(CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorio, 
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return TypedResults.Created($"/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound>> ActualizarGenero(int id, CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorioGeneros,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var existe = await repositorioGeneros.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            var genero = mapper.Map<Genero>(crearGeneroDTO);
            genero.Id = id;
            await repositorioGeneros.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> EliminarGenero(int id, IRepositorioGeneros repositorioGeneros,
            IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorioGeneros.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }
            await repositorioGeneros.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        //Cache para endpoints con controladores:
        //[HttpGet]
        //[OutputCache(Duration = 15)]
        //public IEnumerable<Genero> Get()
        //{
        //    List<Genero> generos =
        //    [
        //        new Genero() { Id = 1, Nombre = "Drama" },
        //        new Genero() { Id = 2, Nombre = "Accion" },
        //        new Genero() { Id = 3, Nombre = "Comedia" },
        //    ];

        //    return generos;
        //}
    }
}
