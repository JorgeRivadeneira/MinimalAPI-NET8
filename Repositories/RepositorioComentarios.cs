using Microsoft.EntityFrameworkCore;
using MinimalAPIPeliculas.Data;
using MinimalAPIPeliculas.Entities;

namespace MinimalAPIPeliculas.Repositories
{
    public class RepositorioComentarios : IRepositorioComentarios
    {
        private readonly ApplicationDbContext context;

        public RepositorioComentarios(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<Comentario>> ObtenerTodos(int peliculaId)
        {
            return await context.Comentarios.Where(x => x.PeliculaId == peliculaId).ToListAsync();
        }

        public async Task<Comentario?> ObtenerPorId(int id)
        {
            return await context.Comentarios.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> Crear(Comentario comentario)
        {
            context.Add(comentario);
            await context.SaveChangesAsync();
            return comentario.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await context.Comentarios.AnyAsync(x => x.Id == id);
        }

        public async Task Actualizar(Comentario comentario)
        {
            context.Update(comentario);
            await context.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await context.Comentarios.Where(x => x.Id == id).ExecuteDeleteAsync();
        }
    }
}
