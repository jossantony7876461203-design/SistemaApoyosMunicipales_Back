using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
    {
        public void Configure(EntityTypeBuilder<Permiso> builder)
        {
            builder.ToTable("permisos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasColumnName("id");

            builder.Property(p => p.Codigo)
                .HasColumnName("codigo")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Modulo)
                .HasColumnName("modulo")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Descripcion)
                .HasColumnName("descripcion");

            // ✅ columnas nuevas
            builder.Property(p => p.Activo)
                .HasColumnName("activo")
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            builder.Property(p => p.DeletedAt)
                .HasColumnName("deleted_at");

            // índices
            builder.HasIndex(p => p.Codigo)
                .IsUnique();

            builder.HasIndex(p => p.Activo)
                .HasFilter("activo = TRUE AND deleted_at IS NULL");
        }
    }
}