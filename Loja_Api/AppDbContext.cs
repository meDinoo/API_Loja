using Loja_Api.Model;
using Loja_Api.Model.Pedido;
using Microsoft.EntityFrameworkCore;

namespace Loja_Api
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Produto> Produtos => Set<Produto>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<PedidoItem> ItensPedido => Set<PedidoItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().Property(produto => produto.Preco).HasPrecision(18, 2);
            modelBuilder.Entity<Pedido>().Property(pedido => pedido.Subtotal).HasPrecision(18, 2);
            modelBuilder.Entity<Pedido>().Property(pedido => pedido.Frete).HasPrecision(18, 2);
            modelBuilder.Entity<Pedido>().Property(pedido => pedido.Total).HasPrecision(18, 2);
            modelBuilder.Entity<PedidoItem>().Property(item => item.PrecoUnitario).HasPrecision(18, 2);

            modelBuilder.Entity<PedidoItem>()
                .HasOne<Pedido>()
                .WithMany()
                .HasForeignKey(item => item.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PedidoItem>()
                .HasOne<Produto>()
                .WithMany()
                .HasForeignKey(item => item.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
