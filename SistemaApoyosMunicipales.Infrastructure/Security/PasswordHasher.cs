using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaApoyosMunicipales.Infrastructure.Security
{
    public sealed class PasswordHasher : IPasswordHasher { 
        public string Hash(string password) { 
            return BCrypt.Net.BCrypt.HashPassword(password); 
        } 
        public bool Verify(string password, string hash) {
            return BCrypt.Net.BCrypt.Verify(password, hash); }
    }
}
