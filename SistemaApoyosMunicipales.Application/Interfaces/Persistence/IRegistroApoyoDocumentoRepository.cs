using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IRegistroApoyoDocumentoRepository
    {
        Task<RegistroApoyoDocumento> ObtenerPorIdAsync(Guid id);
        Task<IEnumerable<RegistroApoyoDocumento>> ObtenerPorRegistroApoyoIdAsync(Guid registroApoyoId);
        Task<PaginatedResult<RegistroApoyoDocumento>> ObtenerTodosAsync(PaginationRequest pagination, Guid? registroApoyoId = null);
        Task AgregarAsync(RegistroApoyoDocumento documento);
        Task AgregarRangeAsync(IEnumerable<RegistroApoyoDocumento> documentos);
        void Actualizar(RegistroApoyoDocumento documento);
        Task EliminarAsync(Guid id);
        Task EliminarPorRegistroApoyoIdAsync(Guid registroApoyoId);
        Task<bool> ExisteAsync(Guid id);
        Task<int> ContarPorRegistroApoyoIdAsync(Guid registroApoyoId);
    }
}
