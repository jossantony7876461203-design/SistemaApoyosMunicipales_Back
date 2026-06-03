using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
    {
        public interface IEmailService
        {
            Task EnviarActivacionAsync(
                string correo,
                string nombre,
                string token
            );
            Task EnviarRecuperacionPasswordAsync(string correo, string nombre, string codigo);
        }
    }


}
