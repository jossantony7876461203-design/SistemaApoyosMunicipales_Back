using SistemaApoyosMunicipales.Domain.Entities.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Persistence
{
    public interface ILogAccesoRepository
    {
        Task RegistrarAsync(LogAcceso log);
    }
}
