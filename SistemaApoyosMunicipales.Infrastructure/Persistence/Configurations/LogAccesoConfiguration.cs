using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class LogAccesoConfiguration : IEntityTypeConfiguration<LogAcceso>
    {
        public void Configure(EntityTypeBuilder<LogAcceso> builder)
        {
            builder.ToTable("log_accesos");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).HasColumnName("id");
            builder.Property(l => l.UsuarioId).HasColumnName("usuario_id");
            builder.Property(l => l.Accion).HasColumnName("accion").HasMaxLength(100).IsRequired();
            builder.Property(l => l.Ip).HasColumnName("ip").HasMaxLength(45);
            builder.Property(l => l.UserAgent).HasColumnName("user_agent");
            builder.Property(l => l.Exitoso).HasColumnName("exitoso").HasDefaultValue(true);
            builder.Property(l => l.Detalle).HasColumnName("detalle");
            builder.Property(l => l.CreatedAt).HasColumnName("created_at");

            builder.HasOne(l => l.Usuario)
                   .WithMany()
                   .HasForeignKey(l => l.UsuarioId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
