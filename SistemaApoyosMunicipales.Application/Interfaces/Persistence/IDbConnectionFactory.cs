using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    /// <summary>
    /// Abstracción para abrir conexiones crudas (usadas por Dapper en
    /// el módulo de reportes). No reemplaza a EF Core / IUnitOfWork,
    /// es exclusivamente para consultas de solo lectura muy dinámicas
    /// donde EF sería más lento o más complicado de armar.
    /// </summary>
    public interface IDbConnectionFactory
    {
        IDbConnection CrearConexion();
    }
}
