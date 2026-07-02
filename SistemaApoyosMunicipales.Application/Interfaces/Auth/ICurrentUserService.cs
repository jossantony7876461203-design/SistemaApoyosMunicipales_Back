using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        bool IsAuthenticated { get; }
    }
}
