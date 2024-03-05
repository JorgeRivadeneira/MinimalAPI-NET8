using MinimalAPIPeliculas.Entities;
using System.ComponentModel.DataAnnotations;

namespace MinimalAPIPeliculas.Models
{
    public class Genero
    {
        public int Id { get; set; }
        //[StringLength(50)] //También se puede hacer lo mismo usando API fluent en el ApplicationDbContext
        public string Nombre { get; set; } = null!; //Perdonar los nulos
        public List<GeneroPelicula> GenerosPelicula{ get; set; } = new List<GeneroPelicula>();
    }
}
