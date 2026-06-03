using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class SubRolPermisoConfiguration : IEntityTypeConfiguration<SubRolPermiso>
    {
        public void Configure(EntityTypeBuilder<SubRolPermiso> builder)
        {
            builder.ToTable("sub_roles_permisos");
            builder.HasKey(sp => new { sp.SubRolId, sp.PermisoId });
            builder.Property(sp => sp.SubRolId).HasColumnName("sub_rol_id");
            builder.Property(sp => sp.PermisoId).HasColumnName("permiso_id");
            builder.Property(sp => sp.PuedeCrear).HasColumnName("puede_crear").HasDefaultValue(false);
            builder.Property(sp => sp.PuedeLeer).HasColumnName("puede_leer").HasDefaultValue(true);
            builder.Property(sp => sp.PuedeEditar).HasColumnName("puede_editar").HasDefaultValue(false);
            builder.Property(sp => sp.PuedeEliminar).HasColumnName("puede_eliminar").HasDefaultValue(false);
            builder.Property(sp => sp.AsignadoAt).HasColumnName("asignado_at");

            builder.HasOne(sp => sp.SubRol)
                   .WithMany(s => s.SubRolesPermisos)
                   .HasForeignKey(sp => sp.SubRolId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sp => sp.Permiso)
                   .WithMany()
                   .HasForeignKey(sp => sp.PermisoId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
