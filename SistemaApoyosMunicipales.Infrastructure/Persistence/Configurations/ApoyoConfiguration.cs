using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Apoyo;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public sealed class ApoyoConfiguration : IEntityTypeConfiguration<Apoyo>
    {
        public void Configure(EntityTypeBuilder<Apoyo> builder)
        {
            builder.ToTable("apoyos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.Codigo)
                .HasColumnName("codigo")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Descripcion)
                .HasColumnName("descripcion");

            builder.Property(x => x.MontoMaximo)
                .HasColumnName("monto_maximo")
                .HasPrecision(18, 2);

            builder.Property(x => x.RequiereValidacion)
                .HasColumnName("requiere_validacion");

            builder.Property(x => x.Activo)
                .HasColumnName("activo");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at");

            builder.Property(x => x.DeletedAt)
                .HasColumnName("deleted_at");

            builder.HasIndex(x => x.Codigo)
                .IsUnique();
        }
    }
}
