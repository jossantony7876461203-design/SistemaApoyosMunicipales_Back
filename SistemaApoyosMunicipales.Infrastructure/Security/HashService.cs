using System.Security.Cryptography;
using System.Text;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;

namespace SistemaApoyosMunicipales.Infrastructure.Security
{
    public sealed class HashService
        : IHashService
    {
        public string GenerarSHA256(string value)
        {
            var bytes = System.Security.Cryptography.SHA256
                .HashData(
                    Encoding.UTF8.GetBytes(value)
                );

            return Convert.ToHexString(bytes);
        }
    }
}