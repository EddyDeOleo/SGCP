using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeCarrito;
using SGCP.Domain.Entities.ModuloDePedido;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Domain.Entities.ModuloDeReporte;
using SGCP.Domain.Entities.ModuloDeUsuarios;

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
    public DbSet<Pedido> Pedido { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(u => u.IdUsuario);
            entity.Property(u => u.IdUsuario).HasColumnName("usuario_id").ValueGeneratedOnAdd();
            entity.Property(u => u.Nombre).HasColumnName("nombre").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Apellido).HasColumnName("apellido").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
            entity.Property(u => u.Password).HasColumnName("password").HasMaxLength(255).IsRequired();

            entity.Property(u => u.FechaCreacion)
                .HasColumnName("fecha_creacion")
                .HasDefaultValueSql("GETDATE()")
                .IsRequired();

            entity.Property(u => u.FechaModificacion)
                .HasColumnName("fecha_modificacion");

            entity.Property(u => u.UsuarioModificacion)
                .HasColumnName("usuario_modificacion");

            entity.Property(u => u.Estatus)
                .HasColumnName("estatus")
                .HasDefaultValue(true)
                .IsRequired();
        });

        modelBuilder.Entity<Cliente>().ToTable("Cliente");
        modelBuilder.Entity<Administrador>().ToTable("Administrador");

        modelBuilder.Ignore<Reporte>();
        modelBuilder.Ignore<Pedido>();
        modelBuilder.Ignore<Producto>();
        modelBuilder.Ignore<Carrito>();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.FechaCreacion = DateTime.Now;
                entry.Entity.Estatus = true;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.FechaModificacion = DateTime.Now;

                entry.Property(nameof(BaseEntity.FechaCreacion)).IsModified = false;

            }
        }
    }
}
