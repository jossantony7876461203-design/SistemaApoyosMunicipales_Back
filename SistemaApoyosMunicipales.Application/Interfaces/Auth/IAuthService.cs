using SistemaApoyosMunicipales.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
        Task RegistrarAsync(RegistroDto dto);
        Task RecuperarPasswordAsync(RecuperarPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task ActivarCuentaAsync(string token);
    }
}
