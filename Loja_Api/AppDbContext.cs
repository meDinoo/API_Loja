using Loja_Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Loja_Api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();

    }
}
