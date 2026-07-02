using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auditoria;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public sealed class AuditoriaConfiguration : IEntityTypeConfiguration<Auditoria>
    {
        public void Configure(EntityTypeBuilder<Auditoria> builder)
        {
            builder.ToTable("auditoria");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.Entidad)
                .HasColumnName("entidad")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.EntidadId)
                .HasColumnName("entidad_id")
                .IsRequired();

            builder.Property(x => x.Accion)
                .HasColumnName("accion")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.ValorAnterior)
                .HasColumnName("valor_anterior")
                .HasMaxLength(100);

            builder.Property(x => x.ValorNuevo)
                .HasColumnName("valor_nuevo")
                .HasMaxLength(100);

            builder.Property(x => x.Comentario)
                .HasColumnName("comentario");

            builder.Property(x => x.UsuarioId)
                .HasColumnName("usuario_id");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            // Relación con Usuario (sin navegación inversa en Usuario para no ensuciar esa entidad)
            builder.HasOne<Domain.Entities.Auth.Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(x => new { x.Entidad, x.EntidadId });
            builder.HasIndex(x => x.UsuarioId);
            builder.HasIndex(x => x.CreatedAt);
        }
    }
}
