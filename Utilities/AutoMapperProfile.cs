using AutoMapper;
using MinimalAPIPeliculas.DTOs;
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
        }
    }
}
