using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Domain.Entities.Apoyo;
using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IApoyoRepository
    {
        Task AgregarAsync(Apoyo apoyo);

        void Actualizar(Apoyo apoyo);                  

        Task<Apoyo?> ObtenerPorIdAsync(Guid id);

        Task<Apoyo?> ObtenerPorIdParaEditarAsync(Guid id);

        Task<Apoyo?> ObtenerPorCodigoAsync(string codigo);

        Task<PaginatedResult<Apoyo>> ObtenerTodosAsync(
            PaginationRequest pagination,
            bool activos = true);

        Task CambiarEstatusAsync(Guid id, bool activo);

        Task EliminarAsync(Guid id);

        Task<List<RegistroApoyoDocumento>>
    ObtenerDocumentosAsync(Guid registroApoyoId);
    }
}
