using Microsoft.EntityFrameworkCore;
using SistemaApoyosMunicipales.Domain.Entities.Auth;
using SistemaApoyosMunicipales.Domain.Entities.Comunidad;
using System;
using System.Collections.Generic;
using System.Text;


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



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica automáticamente todas las clases Configuration de este ensamblado
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
