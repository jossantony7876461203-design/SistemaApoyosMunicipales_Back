using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface ITokenService
    {
        string GenerarTokenSeguro();
      
        string GenerarCodigo6Digitos();
    }
}


