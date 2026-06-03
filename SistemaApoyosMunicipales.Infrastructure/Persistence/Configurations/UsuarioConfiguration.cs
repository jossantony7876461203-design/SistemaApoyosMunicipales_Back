using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuarios");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnName("id");
            builder.Property(u => u.Nombre).HasColumnName("nombre").HasMaxLength(200).IsRequired();
            builder.Property(u => u.Correo).HasColumnName("correo").HasColumnType("citext").HasMaxLength(255).IsRequired();
            builder.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
            builder.Property(u => u.Activo).HasColumnName("activo").HasDefaultValue(false);
            builder.Property(u => u.CorreoVerificado).HasColumnName("correo_verificado").HasDefaultValue(false);
            builder.Property(u => u.CorreoVerificadoAt).HasColumnName("correo_verificado_at");
            builder.Property(u => u.RolId).HasColumnName("rol_id");
            builder.Property(u => u.SubRolId).HasColumnName("sub_rol_id");
            builder.Property(u => u.CreatedAt).HasColumnName("created_at");
            builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
            builder.Property(u => u.UltimoAcceso).HasColumnName("ultimo_acceso");

            builder.HasIndex(u => u.Correo).IsUnique();

            builder.HasOne(u => u.Rol)
                   .WithMany()
                   .HasForeignKey(u => u.RolId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(u => u.SubRol)
                   .WithMany()
                   .HasForeignKey(u => u.SubRolId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
