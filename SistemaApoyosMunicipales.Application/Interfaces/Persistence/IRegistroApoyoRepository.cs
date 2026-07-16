using SistemaApoyosMunicipales.Application.Common.Models;
using SistemaApoyosMunicipales.Domain.Entities.Documentos;
using SistemaApoyosMunicipales.Domain.Entities.RegistroDeApoyos;
using SistemaApoyosMunicipales.Domain.Estados;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IRegistroApoyoRepository
    {
        Task AgregarAsync(RegistroApoyo registro);

        void Actualizar(RegistroApoyo registro);

        Task<RegistroApoyo?> ObtenerPorIdAsync(Guid id);

        Task<RegistroApoyo?> ObtenerPorIdParaEditarAsync(Guid id);

        Task CambiarEstatusAsync(
            Guid id,
            Guid estadoSolicitudId);

        Task EliminarAsync(Guid id);

        Task<PaginatedResult<RegistroApoyo>>
            ObtenerPorComunidadAsync(
                Guid comunidadId,
                PaginationRequest pagination);

        Task<PaginatedResult<RegistroApoyo>>
            ObtenerTodosAsync(
                PaginationRequest pagination);

        Task<bool> ExisteAsync(Guid id);

        Task<bool> ExisteFolioAsync(string folio);

        Task<bool> ExisteFolioEnOtroRegistroAsync(
            string folio,
            Guid registroId);

        Task<List<RegistroApoyoDocumento>>
            ObtenerDocumentosAsync(Guid registroApoyoId);

        Task EliminarDocumentosAsync(Guid registroApoyoId);

        Task AgregarDocumentoAsync(
            RegistroApoyoDocumento documento);

        Task AgregarDocumentosAsync(
            List<RegistroApoyoDocumento> documentos);

        Task<List<EstadoSolicitud>>
            ObtenerEstadosSolicitudAsync();
    }
}