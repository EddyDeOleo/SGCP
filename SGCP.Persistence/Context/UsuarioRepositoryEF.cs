using Microsoft.EntityFrameworkCore;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;

public class SGCPDbContext : DbContext
{
    public SGCPDbContext(DbContextOptions<SGCPDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Administrador> Administradores { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasDiscriminator<string>("TipoUsuario")
            .HasValue<Cliente>("Cliente")
            .HasValue<Administrador>("Administrador");

        modelBuilder.Entity<Usuario>().Property(u => u.Nombre).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<Usuario>().Property(u => u.Apellido).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<Usuario>().Property(u => u.Username).HasMaxLength(50).IsRequired();
        modelBuilder.Entity<Usuario>().Property(u => u.Password).HasMaxLength(255).IsRequired();

        modelBuilder.Entity<Cliente>().HasOne(c => c.Carrito).WithOne().HasForeignKey<Carrito>(c => c.IdCarrito);
    }
}
