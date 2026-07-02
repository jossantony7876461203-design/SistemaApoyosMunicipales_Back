using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Estados;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public sealed class EstadoSolicitudConfiguration : IEntityTypeConfiguration<EstadoSolicitud>
    {
        public void Configure(EntityTypeBuilder<EstadoSolicitud> builder)
        {
            builder.ToTable("estados_solicitud");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.Clave)
                .HasColumnName("clave")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Descripcion)
                .HasColumnName("descripcion");

            builder.Property(x => x.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            builder.HasIndex(x => x.Clave)
                .IsUnique();

            // Navegación inversa configurada del lado de RegistroApoyoConfiguration
        }
    }
}
