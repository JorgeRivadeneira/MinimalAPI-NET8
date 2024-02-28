﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Data.SqlClient.DataClassification;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Migrations;
using MinimalAPIPeliculas.Repositories;

namespace MinimalAPIPeliculas.Endpoints
{
    public static class ComentariosEndpoints
    {
        public static RouteGroupBuilder MapComentarios(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerTodos)
                .CacheOutput(o => o.Expire(TimeSpan.FromSeconds(60)).Tag("comentarios-get").SetVaryByRouteValue(new string[] { "peliculaId" }));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear);

            group.MapPut("/{id:int}", Actualizar);
            group.MapDelete("/{id:int}", Borrar);
            return group;
        }

        static async Task<Results<NoContent, NotFound>> Actualizar(int peliculaId, int id, 
            CrearComentarioDTO crearComentarioDTO , IOutputCacheStore outputCacheStore, 
            IRepositorioComentarios repositorioComentarios, IRepositorioPeliculas repositorioPeliculas, IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            if (!await repositorioComentarios.Existe(id))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.Id = id;
            comentario.PeliculaId = peliculaId;

            await repositorioComentarios.Actualizar(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NotFound();

        }

        static async Task<Results<NoContent, NotFound>> Borrar(int peliculaId, int id, 
            IRepositorioComentarios repositorioComentarios, IOutputCacheStore outputCacheStore)
        {
            if (!await repositorioComentarios.Existe(id))
            {
                return TypedResults.NotFound();
            }
            await repositorioComentarios.Borrar(id);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            return TypedResults.NotFound();
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

        static async Task<Results<Created<ComentarioDTO>, NotFound>> Crear(int peliculaId,
            CrearComentarioDTO crearComentarioDTO, IRepositorioComentarios repositorioComentarios,
            IRepositorioPeliculas repositorioPeliculas, IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            if(!await repositorioPeliculas.Existe(peliculaId))
            {
                return TypedResults.NotFound();
            }

            var comentario = mapper.Map<Comentario>(crearComentarioDTO);
            comentario.PeliculaId = peliculaId;
            var id = await repositorioComentarios.Crear(comentario);
            await outputCacheStore.EvictByTagAsync("comentarios-get", default);
            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
            return TypedResults.Created($"/comentario/{id}", comentarioDTO);

        }
    }
}
