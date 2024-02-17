using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Entities;
using MinimalAPIPeliculas.Models;

namespace MinimalAPIPeliculas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Genero>().Property(p => p.Nombre).HasMaxLength(50); //You can do the same using Data Annotations
            modelBuilder.Entity<Actor>().Property(x => x.Nombre).HasMaxLength(150);
            modelBuilder.Entity<Actor>().Property(x => x.Foto).IsUnicode();

            modelBuilder.Entity<Pelicula>().Property(p => p.Titulo).HasMaxLength(150);
            modelBuilder.Entity<Pelicula>().Property(p => p.Titulo).IsUnicode();
        }
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }

    }
}
