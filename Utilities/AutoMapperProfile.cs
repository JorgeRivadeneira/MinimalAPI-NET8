using AutoMapper;
using MinimalAPIPeliculas.DTOs;
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
        }
    }
}
