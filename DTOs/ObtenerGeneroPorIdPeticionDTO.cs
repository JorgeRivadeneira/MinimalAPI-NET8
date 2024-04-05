using AutoMapper;
using MinimalAPIPeliculas.Repositories;

namespace MinimalAPIPeliculas.DTOs
{
    public class ObtenerGeneroPorIdPeticionDTO
    {
        public int Id { get; set; }
        public IRepositorioGeneros RepositorioGeneros{ get; set; }
        public IMapper Mapper{ get; set; }
    }
}
