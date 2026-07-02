using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.RegistroDeApoyos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public sealed class RegistroApoyoConfiguration : IEntityTypeConfiguration<RegistroApoyo>
    {
        public void Configure(EntityTypeBuilder<RegistroApoyo> builder)
        {
            builder.ToTable("registros_apoyo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");


            builder.Property(x => x.Folio)
        .HasColumnName("folio")
        .HasMaxLength(50)
        .IsRequired();

            builder.Property(x => x.ApoyoId)
                .HasColumnName("apoyo_id")
                .IsRequired();

            builder.Property(x => x.ComunidadId)
                .HasColumnName("comunidad_id")
                .IsRequired();

            builder.Property(x => x.EstadoSolicitudId)
                .HasColumnName("estado_solicitud_id")
                .IsRequired();

            builder.Property(x => x.FechaApoyo)
                .HasColumnName("fecha_apoyo")
                .IsRequired();

            builder.Property(x => x.MontoOtorgado)
                .HasColumnName("monto_otorgado")
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Observaciones)
                .HasColumnName("observaciones");

            builder.Property(x => x.RegistradoPor)
                .HasColumnName("registrado_por")
                .IsRequired();

            builder.Property(x => x.Activo)
                .HasColumnName("activo")
                .HasDefaultValue(true)
                .IsRequired();

            builder.Property(x => x.DeletedAt)
                .HasColumnName("deleted_at");

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("NOW()");

            // ── Relaciones ──────────────────────────────────────

            builder.HasOne(x => x.Apoyo)
                .WithMany()
                .HasForeignKey(x => x.ApoyoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Comunidad)
                .WithMany()
                .HasForeignKey(x => x.ComunidadId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.EstadoSolicitud)
                .WithMany(e => e.RegistrosApoyo)
                .HasForeignKey(x => x.EstadoSolicitudId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Domain.Entities.Auth.Usuario>()
                .WithMany()
                .HasForeignKey(x => x.RegistradoPor)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Documentos)
                .WithOne(d => d.RegistroApoyo)
                .HasForeignKey(d => d.RegistroApoyoId)
                .OnDelete(DeleteBehavior.Cascade);

            // ── Índices ─────────────────────────────────────────

            builder.HasIndex(x => x.ComunidadId);
            builder.HasIndex(x => x.ApoyoId);
            builder.HasIndex(x => x.EstadoSolicitudId);
            builder.HasIndex(x => x.RegistradoPor);
        }
    }
}
