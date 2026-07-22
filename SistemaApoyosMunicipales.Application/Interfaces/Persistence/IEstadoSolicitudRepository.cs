using SistemaApoyosMunicipales.Domain.Estados;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface IEstadoSolicitudRepository
    {
        Task<bool> ExisteClaveAsync(string clave);
        Task<bool> ExisteClaveEnOtroRegistroAsync(string clave, Guid id);
        Task AgregarAsync(EstadoSolicitud estado);
        Task<EstadoSolicitud?> ObtenerPorIdAsync(Guid id);
        Task<List<EstadoSolicitud>> ObtenerTodosAsync();
        Task<bool> ExisteAsync(Guid id);
        Task<bool> TieneRegistrosAsociadosAsync(Guid id);
        Task CambiarEstatusAsync(Guid id, bool activo);
    }
}
