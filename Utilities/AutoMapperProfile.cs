using AutoMapper;
using MinimalAPIPeliculas.DTOs;
using MinimalAPIPeliculas.Endpoints;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Models;

namespace MinimalAPIPeliculas.Utilities
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            //Since: CrearGeneroDTO => Genero
            CreateMap<CrearGeneroDTO, Genero>();

            //Genero => GeneroDTO
            CreateMap<Genero, GeneroDTO>();


            CreateMap<CrearActorDTO, Actor>()
                .ForMember(x => x.Foto, opciones => opciones.Ignore());
            CreateMap<Actor, ActorDTO>();

            CreateMap<CrearPeliculaDTO, Pelicula>()
                .ForMember(x => x.Poster, opciones => opciones.Ignore());
            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(p => p.Generos,
                entidad => entidad.MapFrom(p =>
                p.GenerosPelicula.Select(gp =>
                    new GeneroDTO { Id = gp.GeneroId, Nombre = gp.Genero.Nombre })))
                .ForMember(p => p.Actores, entidad => entidad.MapFrom(p =>
                p.ActoresPelicula.Select(ap =>
                    new ActorPeliculaDTO
                    {
                        Id = ap.ActorId,
                        Nombre = ap.Actor.Nombre,
                        Personaje = ap.Personaje
                    })));


            CreateMap<CrearComentarioDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();

            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();
        }
    }
}
