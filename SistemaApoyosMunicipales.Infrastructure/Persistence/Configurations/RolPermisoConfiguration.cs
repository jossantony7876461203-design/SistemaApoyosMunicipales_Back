using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class RolPermisoConfiguration : IEntityTypeConfiguration<RolPermiso>
    {
        public void Configure(EntityTypeBuilder<RolPermiso> builder)
        {
            builder.ToTable("roles_permisos");
            builder.HasKey(rp => new { rp.RolId, rp.PermisoId });
            builder.Property(rp => rp.RolId).HasColumnName("rol_id");
            builder.Property(rp => rp.PermisoId).HasColumnName("permiso_id");
            builder.Property(rp => rp.AsignadoAt).HasColumnName("asignado_at");

            builder.HasOne(rp => rp.Rol)
                   .WithMany(r => r.RolesPermisos)
                   .HasForeignKey(rp => rp.RolId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.Permiso)
                   .WithMany()
                   .HasForeignKey(rp => rp.PermisoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
