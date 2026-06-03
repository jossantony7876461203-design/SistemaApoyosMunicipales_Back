
using System.Security.Cryptography;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.Infrastructure.Security
{
    public sealed class TokenService
        : ITokenService
    {
        public string GenerarTokenSeguro()
        {
            return Convert.ToHexString(
                RandomNumberGenerator.GetBytes(32)
            );
        }

        public string GenerarCodigo6Digitos()
        {
          
            var bytes = RandomNumberGenerator.GetBytes(4);
            var numero = BitConverter.ToUInt32(bytes, 0) % 1000000;
            return numero.ToString("D6"); 
        }
    }
}

