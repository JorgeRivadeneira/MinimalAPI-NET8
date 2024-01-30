using Microsoft.EntityFrameworkCore;

namespace MinimalAPIPeliculas.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
