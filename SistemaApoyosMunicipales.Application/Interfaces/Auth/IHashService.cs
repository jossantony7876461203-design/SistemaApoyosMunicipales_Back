namespace SistemaApoyosMunicipales.Application.Interfaces.Auth
{
    public interface IHashService
    {
        string GenerarSHA256(string value);
    }
}
