using Microsoft.EntityFrameworkCore;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using Microsoft.Extensions.Logging;

public class SGCPDbContext : DbContext
{
    public SGCPDbContext(DbContextOptions<SGCPDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
    }

    public DbSet<Usuario> Usuario { get; set; }
    public DbSet<Cliente> Cliente { get; set; }
    public DbSet<Administrador> Administrador { get; set; }
    public DbSet<Carrito> Carrito { get; set; }
    public DbSet<Pedido> Pedido { get; set; }
    public DbSet<Reporte> Reporte { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===============================
        // Usuario (Tabla base)
        // ===============================
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(u => u.IdUsuario);
            entity.Property(u => u.IdUsuario)
                .HasColumnName("usuario_id")
                .ValueGeneratedOnAdd();

            entity.Property(u => u.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Apellido).HasColumnName("apellido").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
        });

        // ===============================
        // Cliente (TPT)
        // ===============================
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Cliente");
        });

        // ===============================
        // Administrador (TPT)
        // ===============================
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.ToTable("Administrador");
        });
        // ===============================
        // Carrito
        // ===============================
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.ToTable("Carrito");
            entity.HasKey(c => c.IdCarrito);
            entity.Property(c => c.IdCarrito)
                .HasColumnName("carrito_id")
                .ValueGeneratedOnAdd();

            entity.Property(c => c.ClienteId).HasColumnName("cliente_id");

            entity.HasOne(c => c.Cliente)
                .WithOne(cl => cl.Carrito)
                .HasForeignKey<Carrito>(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===============================
        // Pedido
        // ===============================
        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.ToTable("Pedido");
            entity.HasKey(p => p.IdPedido);
            entity.Property(p => p.IdPedido)
                .HasColumnName("pedido_id")
                .ValueGeneratedOnAdd();

            entity.Property(p => p.ClienteId).HasColumnName("cliente_id");
            entity.Property(p => p.CarritoId).HasColumnName("carrito_id");
            entity.Property(p => p.FechaCreacion).HasColumnName("fecha");
            entity.Property(p => p.Estado).HasColumnName("estado").HasMaxLength(30).IsRequired();
            entity.Property(p => p.Total).HasColumnName("total").HasColumnType("decimal(10,2)").IsRequired();

            entity.HasOne(p => p.Cliente)
                .WithMany(c => c.HistorialPedidos)
                .HasForeignKey(p => p.ClienteId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(p => p.Carrito)
                .WithMany()
                .HasForeignKey(p => p.CarritoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===============================
        // Reporte
        // ===============================
        modelBuilder.Entity<Reporte>(entity =>
        {
            entity.ToTable("Reporte");
            entity.HasKey(r => r.IdReporte);
            entity.Property(r => r.IdReporte)
                .HasColumnName("reporte_id")
                .ValueGeneratedOnAdd();

            entity.Property(r => r.AdminId).HasColumnName("admin_id");
            entity.Property(r => r.FechaCreacion).HasColumnName("fecha");
            entity.Property(r => r.TotalVentas).HasColumnName("total_ventas");
            entity.Property(r => r.TotalPedidos).HasColumnName("total_pedidos");

            entity.HasOne<Administrador>()
                .WithMany()
                .HasForeignKey(r => r.AdminId)
                .HasPrincipalKey(a => a.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
