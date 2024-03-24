using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Data.SqlClient.DataClassification;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Filtros;
using MinimalAPIPeliculas.Migrations;
using MinimalAPIPeliculas.Repositories;
using MinimalAPIPeliculas.Servicios;

namespace MinimalAPIPeliculas.Endpoints
{
    public static class ComentariosEndpoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(o => o.Expire(TimeSpan.FromSeconds(60)).Tag("comentarios-get").SetVaryByRouteValue(new string[] { "peliculaId" }));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>().RequireAuthorization();

            group.MapPut("/{id:int}", Actualizar).AddEndpointFilter<FiltroValidaciones<CrearComentarioDTO>>().RequireAuthorization();
            group.MapDelete("/{id:int}", Borrar).RequireAuthorization();
            return group;
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Actualizar(int peliculaId, int id, 
            CrearComentarioDTO crearComentarioDTO , IOutputCacheStore outputCacheStore, 
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas, IServicioUsuario servicioUsuario)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarioDB = await repositorioComentarios.ObtenerPorId(id);
            if(comentarioDB is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuario.ObtenerUsuario();
            if(usuario is null)
            {
                return TypedResults.NotFound();
            }

            if(comentarioDB.UsuarioId != usuario.Id)
            {
                return TypedResults.Forbid();
            }

            if (!await repositorioComentarios.Existe(id))
            {
                return TypedResults.NotFound();
            }

            comentarioDB.Cuerpo = crearComentarioDTO.Cuerpo;

            await repositorioComentarios.Actualizar(comentarioDB);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();

        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Borrar(int peliculaId, int id, 
            IRepositorioComentarios repositorioComentarios, IOutputCacheStore outputCacheStore,
            IServicioUsuario servicioUsuario)
        {

            var comentarioDB = await repositorioComentarios.ObtenerPorId(id);
            if (comentarioDB is null)
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuario.ObtenerUsuario();
            if (usuario is null)
            {
                return TypedResults.NotFound();
            }

            if (comentarioDB.UsuarioId != usuario.Id)
            {
                return TypedResults.Forbid();
            }

            await repositorioComentarios.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<Ok<List<ComentarioDTO>>, NotFound>> ObtenerTodos(int peliculaId,
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas,
            IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentarios = await repositorioComentarios.ObtenerTodos(peliculaId);
            var comentarioDTO = mapper.Map<List<ComentarioDTO>>(comentarios);
            return TypedResults.Ok(comentarioDTO);
        }

        static async Task<Results<Ok<ComentarioDTO>, NotFound>> ObtenerPorId(int peliculaId, int id,
            IRepositorioComentarios repositorioComentarios, IMapper mapper)
        {
            var comentario = await repositorioComentarios.ObtenerPorId(id);

            if(comentario == null)
            {
                return TypedResults.NotFound();
            }

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return TypedResults.Ok(comentarioDTO);
        }

        static async Task<Results<Created<ComentarioDTO>, NotFound, BadRequest<string>>> Crear(int peliculaId,
            CrearComentarioDTO crearComentarioDTO, IRepositorioComentarios repositorioComentarios,
            IRepositorioPeliculas repositorioPeliculas, IMapper mapper, IOutputCacheStore outputCacheStore,
            IServicioUsuario servicioUsuario)
        {
            if(!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var usuario = await servicioUsuario.ObtenerUsuario();

            if(usuario is null) { return TypedResults.BadRequest("Usuario no Encontrado"); }           

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.UsuarioId = usuario.Id;
            comentario.PeliculaId = peliculaId;
            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Created($"/comentario/{id}", comentarioDTO);

        }
    }
}
