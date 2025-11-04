
using Microsoft.AspNetCore.Http;
using SGCP.Application.Interfaces;
using SGCP.Infraestructure.Security;
using System.Security.Claims;

namespace SGCP.Application.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtTokenService _jwtTokenService;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, JwtTokenService jwtTokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtTokenService = jwtTokenService;
        }

        public int? GetUserId()
        {
            var userIdClaim = GetClaimValue("UserId") ?? GetClaimValue(ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
                return userId;

            return null;
        }

        public string GetUserName()
        {
            return GetClaimValue(ClaimTypes.Name) ?? GetClaimValue("Username") ?? string.Empty;
        }

        /// <summary>
        /// Obtiene un claim específico desde el contexto HTTP o, si no existe, lo extrae validando el token directamente.
        /// </summary>
        private string? GetClaimValue(string claimType)
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(claimType);
            if (claim != null)
                return claim.Value;

            // Si no existe en el contexto, intenta leer manualmente el token
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .FirstOrDefault()?.Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                var principal = _jwtTokenService.ValidateToken(token);
                return principal?.FindFirst(claimType)?.Value;
            }

            return null;
        }
    }
}
