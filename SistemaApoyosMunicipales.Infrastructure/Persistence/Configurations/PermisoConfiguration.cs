using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class PermisoConfiguration : IEntityTypeConfiguration<Permiso>
    {
        public void Configure(EntityTypeBuilder<Permiso> builder)
        {
            builder.ToTable("permisos");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.Codigo).HasColumnName("codigo").HasMaxLength(100).IsRequired();
            builder.Property(p => p.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
            builder.Property(p => p.Modulo).HasColumnName("modulo").HasMaxLength(100).IsRequired();
            builder.Property(p => p.Descripcion).HasColumnName("descripcion");
            builder.HasIndex(p => p.Codigo).IsUnique();
        }
    }
}
