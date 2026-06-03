namespace SistemaApoyosMunicipales.Domain.Exceptions;

public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string mensaje) : base(mensaje) { }
}

public sealed class NotFoundException : Exception
{
    public NotFoundException(string mensaje) : base(mensaje) { }
}

public sealed class ValidationException : Exception
{
    public ValidationException(string mensaje) : base(mensaje) { }
}