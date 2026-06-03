using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class RolConfiguration : IEntityTypeConfiguration<Rol>
    {
        public void Configure(EntityTypeBuilder<Rol> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("id");
            builder.Property(r => r.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(r => r.Descripcion).HasColumnName("descripcion");
            builder.Property(r => r.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(r => r.CreatedAt).HasColumnName("created_at");
            builder.HasIndex(r => r.Nombre).IsUnique();
        }
    }
}
