using Microsoft.EntityFrameworkCore;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;

public class SGCPDbContext : DbContext
{
    public SGCPDbContext(DbContextOptions<SGCPDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuario { get; set; }  // Aquí estarán Usuario, Cliente y Administrador
    public DbSet<Carrito> Carrito { get; set; }
    public DbSet<Pedido> Pedido { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===============================
        // Usuario + herencia (TPH)
        // ===============================
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");

            // PK
            entity.HasKey(u => u.IdUsuario);
            entity.Property(u => u.IdUsuario).HasColumnName("usuario_id");

            // Propiedades
            entity.Property(u => u.Nombre).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Apellido).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Username).HasMaxLength(50).IsRequired();
            entity.Property(u => u.Password).HasMaxLength(255).IsRequired();
        });

        // Discriminador para TPH
        modelBuilder.Entity<Usuario>()
            .HasDiscriminator<string>("TipoUsuario")
            .HasValue<Usuario>("Usuario")
            .HasValue<Cliente>("Cliente")
            .HasValue<Administrador>("Administrador");

        // ===============================
        // Carrito
        // ===============================
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.ToTable("Carrito");
            entity.HasKey(c => c.IdCarrito);
            entity.Property(c => c.IdCarrito).HasColumnName("carrito_id");

            // Relación uno a uno con Cliente
            entity.HasOne(c => c.Cliente)
                  .WithOne(cl => cl.Carrito)
                  .HasForeignKey<Carrito>(c => c.IdCarrito)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ===============================
        // Pedido
        // ===============================
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("Pedido");
            entity.HasKey(p => p.IdPedido);
            entity.Property(p => p.IdPedido).HasColumnName("pedido_id");

            // FK a Cliente
            entity.HasOne(p => p.Cliente)
                  .WithMany(c => c.HistorialPedidos)
                  .HasForeignKey("usuario_id")  // FK apunta a usuario_id de Cliente
                  .IsRequired()
                  .OnDelete(DeleteBehavior.Cascade);

            // FK a Carrito (opcional)
            entity.HasOne(p => p.Carrito)
                  .WithOne()
                  .HasForeignKey<Pedido>(p => p.IdPedido)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
