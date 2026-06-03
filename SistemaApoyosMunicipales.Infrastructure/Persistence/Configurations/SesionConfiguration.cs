using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence.Configurations
{
    public class SesionConfiguration : IEntityTypeConfiguration<Sesion>
    {
        public void Configure(EntityTypeBuilder<Sesion> builder)
        {
            builder.ToTable("sesiones");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).HasColumnName("id");
            builder.Property(s => s.UsuarioId).HasColumnName("usuario_id");
            builder.Property(s => s.TokenHash).HasColumnName("token_hash").HasMaxLength(255).IsRequired();
            builder.Property(s => s.Ip).HasColumnName("ip").HasMaxLength(45);
            builder.Property(s => s.UserAgent).HasColumnName("user_agent");
            builder.Property(s => s.ExpiraAt).HasColumnName("expira_at").IsRequired();
            builder.Property(s => s.CreatedAt).HasColumnName("created_at");

            builder.HasOne(s => s.Usuario)
                   .WithMany()
                   .HasForeignKey(s => s.UsuarioId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
