using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Filtros;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Servicios;

namespace MinimalAPIPeliculas.Endpoints
{
    public static class PeliculasEndpoint
    {
        private static readonly string contenedor = "peliculas";
        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapPost("/", Crear).DisableAntiforgery().RequireAuthorization("isAdmin");
            group.MapGet("/", Obtener).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("peliculas-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>().RequireAuthorization("isAdmin");
            group.MapDelete("/{id:int}", Borrar).RequireAuthorization("isAdmin");
            group.MapPost("/{id:int}/asignarGeneros", AsignarGeneros).AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>().RequireAuthorization("isAdmin");
            //group.MapPost("/{id:int}/asignarActores", AsignarActores);
            return group;
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorioPeliculas,
            IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);
            if(crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Almacenar(contenedor, crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }

            var id = await repositorioPeliculas.Crear(pelicula);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Created($"/peliculas/{id}", peliculaDTO);
        }

        static async Task<Ok<List<PeliculaDTO>>> Obtener(IRepositorioPeliculas repositorioPeliculas, 
            IMapper mapper, int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacion = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var peliculas = await repositorioPeliculas.ObtenerTodos(paginacion);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);
            return TypedResults.Ok(peliculasDTO);
        }

        static async Task<Results<Ok<PeliculaDTO>, NotFound>> ObtenerPorId(int id, IRepositorioPeliculas repositorioPeliculas, IMapper mapper) 
        {
            var pelicula = await repositorioPeliculas.ObtenerPorId(id);
            if(pelicula is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return TypedResults.Ok(peliculaDTO);
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int id, [FromForm]CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorioPeliculas,
            IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            var peliculaDB = await repositorioPeliculas.ObtenerPorId(id);
            if(peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaParaActualizar = mapper.Map<Pelicula>(crearPeliculaDTO);
            peliculaParaActualizar.Id = id;
            peliculaParaActualizar.Poster = peliculaDB.Poster;

            if(crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Editar(peliculaParaActualizar.Poster, contenedor, crearPeliculaDTO.Poster);
                peliculaParaActualizar.Poster = url;
            }

            await repositorioPeliculas.Actualizar(peliculaParaActualizar);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Borrar(int id, IRepositorioPeliculas repositorio,
            IOutputCacheStore outputCacheStore, IAlmacenadorArchivos almacenadorArchivos)
        {
            var peliculaDB = await repositorio.ObtenerPorId(id);
            if(peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(peliculaDB.Poster, contenedor);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id, List<int> generosIds, IRepositorioPeliculas repositorioPeliculas,
            IRepositorioGeneros repositorioGeneros)
        {
            if(!await repositorioPeliculas.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var generosExistentes = new List<int>();
            if(generosIds.Count != 0)
            {
                generosExistentes = await repositorioGeneros.Existen(generosIds);
            }

            if(generosExistentes.Count != generosIds.Count)
            {
                var generosNoExistentes = generosIds.Except(generosExistentes);
                return TypedResults.BadRequest($"Los generos de id {string.Join(",", generosNoExistentes)} no existen");
            }

            await repositorioPeliculas.AsignarGeneros(id, generosExistentes);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AsignarActores(int id, List<AsignarActorPeliculaDTO> actoresDTO,
            IRepositorioPeliculas repositorioPeliculas, IRepositorioActores repositorioActores, IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(id)) { return TypedResults.NotFound(); }

            var actoresExistentes = new List<int>();
            var actoresIds = actoresDTO.Select(a => a.ActorId).ToList();

            if (actoresDTO.Count != 0)
            {
                actoresExistentes = await repositorioActores.Existen(actoresIds);
            }

            if (actoresExistentes.Count != actoresDTO.Count)
            {
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);

                return TypedResults.BadRequest($"Los actores de id {string.Join(",", actoresExistentes)} no existen");
            }

            var actores = mapper.Map<List<ActorPelicula>>(actoresDTO);
            await repositorioPeliculas.AsignarActores(id, actores);
            return TypedResults.NoContent();
        }
    }
}
