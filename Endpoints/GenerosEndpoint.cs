﻿using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Filtros;
using MinimalAPIPeliculas.Models;
using MinimalAPIPeliculas.Repositories;

namespace MinimalAPIPeliculas.Endpoints
{
    public static class GenerosEndpoint
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));
                //.RequireAuthorization();
            group.MapGet("/{id:int}", ObtenerGeneroPorId).AddEndpointFilter<FiltroDePrueba>();
            group.MapPost("/", CrearGenero).AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>().RequireAuthorization("isAdmin");

            group.MapPut("/{id:int}", ActualizarGenero)
                .AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>()
                .RequireAuthorization("isAdmin")
                .WithOpenApi(opciones =>
                {
                    opciones.Summary = "Actualizar un género";
                    opciones.Description = "Con éste endpoint podemos actualizar un género";
                    opciones.Parameters[0].Description = "El id del género a actualizar";
                    opciones.RequestBody.Description = "El género que se desea actualizar";

                    return opciones;
                });

            group.MapDelete("/", EliminarGenero).RequireAuthorization("isAdmin");

            return group;
        }

        static async Task<Ok<List<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio, 
            IMapper mapper, ILoggerFactory loggerFactory)
        {
            var tipo = typeof(GenerosEndpoint);
            var logger = loggerFactory.CreateLogger(tipo.FullName!);
            logger.LogInformation("Obteniendo el listado de géneros");

            var generos = await repositorio.ObtenerTodos();
            //var generosDTO = generos.Select(x => new GeneroDTO { Id = x.Id, Nombre = x.Nombre }).ToList();
            var generosDTO = mapper.Map<List<GeneroDTO>>(generos);  //List<Genero> => List<GeneroDTO>
            return TypedResults.Ok(generosDTO);
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId([AsParameters]ObtenerGeneroPorIdPeticionDTO modelo)
        {
            var genero = await modelo.RepositorioGeneros.ObtenerPorId(modelo.Id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }
            var generoDTO = modelo.Mapper.Map<GeneroDTO>(genero);
            return TypedResults.Ok(generoDTO);
        }

        static async Task<Results<Created<GeneroDTO>, ValidationProblem>> CrearGenero(CrearGeneroDTO crearGeneroDTO, [AsParameters]CrearGeneroPeticionDTO modelo)
        {
            var genero = modelo.Mapper.Map<Genero>(crearGeneroDTO);
            var id = await modelo.Repositorio.Crear(genero);
            await modelo.OutputCacheStore.EvictByTagAsync("generos-get", default);
            var generoDTO = modelo.Mapper.Map<GeneroDTO>(genero);
            return TypedResults.Created($"/{id}", generoDTO);
        }

        static async Task<Results<NoContent, NotFound, ValidationProblem>> ActualizarGenero(int id, CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorioGeneros,
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
