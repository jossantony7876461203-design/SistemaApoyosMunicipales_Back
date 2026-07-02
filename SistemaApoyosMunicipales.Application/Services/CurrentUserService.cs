using Microsoft.AspNetCore.Http;
using SistemaApoyosMunicipales.Application.Interfaces.Auth;
using System;
using System.Security.Claims;

namespace SistemaApoyosMunicipales.Infrastructure.Auth.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var value = _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst(ClaimTypes.NameIdentifier)?
                    .Value;

                return Guid.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }

        public string? UserName =>
            _httpContextAccessor
                .HttpContext?
                .User?
                .Identity?
                .Name;

        public bool IsAuthenticated =>
            _httpContextAccessor
                .HttpContext?
                .User?
                .Identity?
                .IsAuthenticated ?? false;
    }
}