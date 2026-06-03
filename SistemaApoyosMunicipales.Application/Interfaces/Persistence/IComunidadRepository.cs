using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Domain.Entities.Comunidad;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IComunidadRepository
    {
        Task<Comunidad?> ObtenerPorIdAsync(Guid id);

        Task<Comunidad?> ObtenerPorClaveInternaAsync(string claveInterna);
        Task<Comunidad?> ObtenerPorIdParaEditarAsync(Guid id);
        Task<PaginatedResult<Comunidad>> ObtenerTodasAsync(
         PaginationRequest pagination,
         bool activos = true);

        Task AgregarAsync(Comunidad comunidad);

        void Actualizar(Comunidad comunidad);


    }
    
}
