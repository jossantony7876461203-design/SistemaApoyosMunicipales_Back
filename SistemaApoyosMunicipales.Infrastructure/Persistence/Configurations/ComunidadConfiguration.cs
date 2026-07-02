using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Comunidad;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public sealed class ComunidadConfiguration : IEntityTypeConfiguration<Comunidad>
    {
        public void Configure(EntityTypeBuilder<Comunidad> builder)
        {
            // 1. Nombre de la tabla en PostgreSQL
            builder.ToTable("comunidades");

            // 2. Clave primaria (UUID)
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id)
                   .HasColumnName("id")
                   .HasDefaultValueSql("uuid_generate_v4()");

            // 3. Propiedades atómicas de negocio
            builder.Property(c => c.ClaveInterna)
                   .HasColumnName("clave_interna")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(c => c.Nombre)
                   .HasColumnName("nombre")
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(c => c.CodigoPostal)
                   .HasColumnName("codigo_postal")
                   .HasMaxLength(10)
                   .IsRequired();

            builder.Property(c => c.Delegado)
                   .HasColumnName("delegado")
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.Property(c => c.TelefonoDelegado)
                   .HasColumnName("telefono_delegado")
                   .HasMaxLength(20)
                   .IsRequired(false);

            // 4. Propiedades de Control, Auditoría y Soft Delete
            builder.Property(c => c.Activo)
                   .HasColumnName("activo")
                   .HasDefaultValue(true)
                   .IsRequired();

            builder.Property(c => c.DeletedAt)
                   .HasColumnName("deleted_at")
                   .IsRequired(false);

            builder.Property(c => c.CreatedAt)
                   .HasColumnName("created_at")
                   .HasDefaultValueSql("NOW()")
                   .ValueGeneratedOnAdd();

            builder.Property(c => c.UpdatedAt)
                   .HasColumnName("updated_at")
                   .HasDefaultValueSql("NOW()")
                   .ValueGeneratedOnUpdate();

            // 5. Restricción de Unicidad para la Clave de Negocio
            builder.HasIndex(c => c.ClaveInterna)
                   .HasDatabaseName("uq_comunidades_clave")
                   .IsUnique();

            // 6. Índice Operacional Adicional
            builder.HasIndex(c => new { c.Id, c.ClaveInterna })
                   .HasDatabaseName("idx_comunidades_operacionales")
                   .HasFilter("activo = true");

            builder.Property(c => c.DelegadoIneUrl)
    .HasColumnName("delegado_ine_url")
    .HasMaxLength(500);

            builder.Property(c => c.DelegadoInePubId)
                .HasColumnName("delegado_ine_pub_id")
                .HasMaxLength(200);
        }
    }
}