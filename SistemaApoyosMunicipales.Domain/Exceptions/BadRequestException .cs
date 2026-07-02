using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Domain.Exceptions
{
    public sealed class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
