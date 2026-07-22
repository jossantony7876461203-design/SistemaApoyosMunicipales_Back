using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using System;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public sealed class RegistroApoyoDocumentoConfiguration
         : IEntityTypeConfiguration<RegistroApoyoDocumento>
    {
        public void Configure(EntityTypeBuilder<RegistroApoyoDocumento> builder)
        {
            builder.ToTable("registro_apoyo_documentos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id");

            builder.Property(x => x.RegistroApoyoId)
                .HasColumnName("registro_apoyo_id")
                .IsRequired();

            builder.Property(x => x.TipoDocumento)
                .HasColumnName("tipo_documento")
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(x => x.NombreArchivo)
                .HasColumnName("nombre_archivo")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(x => x.Url)
                .HasColumnName("url")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.PublicId)
                .HasColumnName("public_id")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            builder.Property(x => x.Monto)
                .HasColumnName("monto")
                .HasColumnType("numeric(18,2)")
                .IsRequired()
                .HasDefaultValue(0);

            // ✅ DESCRIPCION - CORREGIDO
            builder.Property(x => x.Descripcion)
                .HasColumnName("descripcion")
                .HasMaxLength(500)
                .IsRequired(false);  // Permite NULL

            // ✅ FACTURADO - CORREGIDO
            builder.Property(x => x.Facturado)
                .HasColumnName("facturado")
                .IsRequired()
                .HasDefaultValue(false);

            // ✅ METODO PAGO - CORREGIDO (antes estaba mal configurado)
            builder.Property(x => x.MetodoPago)
                .HasColumnName("metodo_pago")
                .HasMaxLength(20)
                .IsRequired(false);  // Permite NULL

            // ✅ FECHA FACTURADO - CORREGIDO
            builder.Property(x => x.FechaFacturado)
                .HasColumnName("fecha_facturado")
                .IsRequired(false)  // Permite NULL
                .HasDefaultValueSql("NOW()");

            // Índices
            builder.HasIndex(x => x.RegistroApoyoId);
        }
    }
}