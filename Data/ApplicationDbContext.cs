using CiudadDeportivaTudela.Models;
using Microsoft.EntityFrameworkCore;

namespace CiudadDeportivaTudela.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Socio> Socios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Socio>(entity =>
        {
            entity.ToTable("socios");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NumeroSocio).HasColumnName("numero_socio");
            entity.Property(e => e.Dni).HasColumnName("dni");
            entity.Property(e => e.Nombre).HasColumnName("nombre");
            entity.Property(e => e.Apellidos).HasColumnName("apellidos");
            entity.Property(e => e.Telefono).HasColumnName("telefono");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.UrlFoto).HasColumnName("url_foto");
            entity.Property(e => e.PinHash).HasColumnName("pin_hash");
            entity.Property(e => e.PatronHash).HasColumnName("patron_hash");
            entity.Property(e => e.Categoria).HasColumnName("categoria");
            entity.Property(e => e.Cargo).HasColumnName("cargo");
            entity.Property(e => e.NumeroCuenta).HasColumnName("numero_cuenta");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.FechaAlta).HasColumnName("fecha_alta");

            entity.HasIndex(e => e.NumeroSocio).IsUnique();
        });
    }
}
