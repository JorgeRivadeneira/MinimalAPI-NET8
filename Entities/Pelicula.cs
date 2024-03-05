using MinimalAPIPeliculas.Endpoints;

namespace MinimalAPIPeliculas.Entities
{
    public class Pelicula
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = null!;
        public bool EnCines { get; set; } 
        public DateTime FechaLanzamiento { get; set; }
        public string? Poster { get; set; }
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<GeneroPelicula> GenerosPelicula { get; set; } = new List<GeneroPelicula>();
        public List<ActorPelicula> ActoresPelicula { get; set; } = new List<ActorPelicula>();

    }
}
