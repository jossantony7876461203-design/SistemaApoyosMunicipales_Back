using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class TokenVerificacionConfiguration : IEntityTypeConfiguration<TokenVerificacion>
    {
        public void Configure(EntityTypeBuilder<TokenVerificacion> builder)
        {
            builder.ToTable("tokens_verificacion");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.UsuarioId).HasColumnName("usuario_id");
            builder.Property(x => x.TokenHash)
             .HasColumnName("token_hash")
             .HasMaxLength(255)
             .IsRequired();
            builder.Property(t => t.Tipo).HasColumnName("tipo").HasMaxLength(30).IsRequired();
            builder.Property(t => t.ExpiraAt).HasColumnName("expira_at").IsRequired();
            builder.Property(t => t.Usado).HasColumnName("usado").HasDefaultValue(false);
            builder.Property(t => t.CreatedAt).HasColumnName("created_at");

            builder.HasIndex(t => t.TokenHash).IsUnique();

            builder.HasOne(t => t.Usuario)
                   .WithMany()
                   .HasForeignKey(t => t.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
