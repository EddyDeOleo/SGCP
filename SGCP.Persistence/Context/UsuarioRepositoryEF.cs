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
        // Usuario (Tabla base)
        // ===============================
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(u => u.IdUsuario);
            entity.Property(u => u.IdUsuario)
                .HasColumnName("usuario_id")
                .ValueGeneratedOnAdd();  // ✅ Agregar esto para IDENTITY

            entity.Property(u => u.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Apellido).HasColumnName("apellido").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
        });

        // ===============================
        // Cliente (TPT) - CONFIGURACIÓN CORREGIDA
        // ===============================
        modelBuilder.Entity<Cliente>(entity =>
        {
            // Especificar tabla separada para TPT
            entity.ToTable("Cliente", tb =>
            {
                // Configurar que cliente_id es FK a usuario_id
                tb.HasCheckConstraint("FK_Cliente_Usuario", "cliente_id IS NOT NULL");
            });

            // NO mapees IdUsuario a cliente_id aquí
            // La relación se maneja automáticamente por TPT
        });

        // ===============================
        // Administrador (TPT)
        // ===============================
        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.ToTable("Administrador");
        });

        // ===============================
        // Carrito - CONFIGURACIÓN CORREGIDA
        // ===============================
        modelBuilder.Entity<Carrito>(entity =>
        {
            entity.ToTable("Carrito");
            entity.HasKey(c => c.IdCarrito);
            entity.Property(c => c.IdCarrito).HasColumnName("carrito_id");

            // Aquí está el problema: cliente_id debe referenciar la PK de Usuario,
            // que Cliente hereda, NO cliente_id directamente
            entity.Property<int>("ClienteId").HasColumnName("cliente_id");

            entity.HasOne(c => c.Cliente)
                .WithOne(cl => cl.Carrito)
                .HasForeignKey<Carrito>("ClienteId")
                .HasPrincipalKey<Cliente>(cl => cl.IdUsuario)  // ✅ IdUsuario (heredado)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===============================
        // Pedido - CONFIGURACIÓN CORREGIDA
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

            entity.HasOne(p => p.Cliente)
                .WithMany(c => c.HistorialPedidos)
                .HasForeignKey(p => p.ClienteId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);  // ✅ Cambiar de Restrict a Cascade

            entity.HasOne(p => p.Carrito)
                .WithMany()
                .HasForeignKey(p => p.CarritoId)
                .OnDelete(DeleteBehavior.SetNull);  // ✅ O SetNull si el carrito es nullable
        });
    }
}
