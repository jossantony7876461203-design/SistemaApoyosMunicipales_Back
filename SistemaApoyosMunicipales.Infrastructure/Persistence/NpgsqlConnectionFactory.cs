using Microsoft.Extensions.Configuration;
using Npgsql;
using SistemaApoyosMunicipales.Application.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Persistence
{
    public sealed class NpgsqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public NpgsqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IDbConnection CrearConexion()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
