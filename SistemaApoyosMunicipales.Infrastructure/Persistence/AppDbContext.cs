using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Domain.Entities.Apoyo;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Domain.Entities.Comunidad;
using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using SistemaApoyosMunicipales.Domain.Entities.RegistroDeApoyos;
using SistemaApoyosMunicipales.Domain.Estados;


namespace SistemaApoyosMunicipales.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tablas
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Rol> Roles => Set<Rol>();
        public DbSet<SubRol> SubRoles => Set<SubRol>();
        public DbSet<Permiso> Permisos => Set<Permiso>();
        public DbSet<RolPermiso> RolesPermisos => Set<RolPermiso>();
        public DbSet<SubRolPermiso> SubRolesPermisos => Set<SubRolPermiso>();
        public DbSet<TokenVerificacion> TokensVerificacion => Set<TokenVerificacion>();
        public DbSet<Sesion> Sesiones => Set<Sesion>();
        public DbSet<LogAcceso> LogAccesos => Set<LogAcceso>();
        public DbSet<Comunidad> Comunidades => Set<Comunidad>();


        public DbSet<RegistroApoyo> RegistroApoyos => Set<RegistroApoyo>();
        public DbSet<RegistroApoyoDocumento> RegistroApoyoDocumentos => Set<RegistroApoyoDocumento>();
        public DbSet<EstadoSolicitud> EstadosSolicitud { get; set; }

        public DbSet<Apoyo> Apoyos=> Set<Apoyo>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica automáticamente todas las clases Configuration de este ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
