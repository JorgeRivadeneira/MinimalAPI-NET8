using Microsoft.EntityFrameworkCore;
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
        }
        public DbSet<Genero> Generos { get; set; }

    }
}
