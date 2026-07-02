using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class RolPermisoConfiguration : IEntityTypeConfiguration<RolPermiso>
    {
        public void Configure(EntityTypeBuilder<RolPermiso> builder)
        {
            builder.ToTable("roles_permisos");

            builder.HasKey(rp => new { rp.RolId, rp.PermisoId });

            builder.Property(rp => rp.RolId)
                .HasColumnName("rol_id");

            builder.Property(rp => rp.PermisoId)
                .HasColumnName("permiso_id");

            builder.Property(rp => rp.AsignadoAt)
                .HasColumnName("asignado_at")
                .HasDefaultValueSql("NOW()");

            builder.HasOne(rp => rp.Rol)
                .WithMany(r => r.RolesPermisos)
                .HasForeignKey(rp => rp.RolId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rp => rp.Permiso)
                .WithMany(p => p.RolesPermisos)  // ✅ apunta a la colección
                .HasForeignKey(rp => rp.PermisoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}