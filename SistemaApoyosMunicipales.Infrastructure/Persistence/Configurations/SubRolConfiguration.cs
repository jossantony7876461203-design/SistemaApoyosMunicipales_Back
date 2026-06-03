using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class SubRolConfiguration : IEntityTypeConfiguration<SubRol>
    {
        public void Configure(EntityTypeBuilder<SubRol> builder)
        {
            builder.ToTable("sub_roles");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("id");
            builder.Property(s => s.RolId).HasColumnName("rol_id");
            builder.Property(s => s.Nombre).HasColumnName("nombre").HasMaxLength(100).IsRequired();
            builder.Property(s => s.Descripcion).HasColumnName("descripcion");
            builder.Property(s => s.Activo).HasColumnName("activo").HasDefaultValue(true);
            builder.Property(s => s.CreatedAt).HasColumnName("created_at");

            builder.HasOne(s => s.Rol)
                   .WithMany(r => r.SubRoles)
                   .HasForeignKey(s => s.RolId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => new { s.RolId, s.Nombre }).IsUnique();
        }
    }
}
