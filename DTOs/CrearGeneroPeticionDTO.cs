using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIPeliculas.Repositories;

namespace MinimalAPIPeliculas.DTOs
{
    public class CrearGeneroPeticionDTO
    {
        public IRepositorioGeneros Repositorio { get; set; }
        public IOutputCacheStore OutputCacheStore { get; set; }
        public IMapper Mapper { get; set; }
    }
}
