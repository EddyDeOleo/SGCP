
using Microsoft.AspNetCore.Http;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Infraestructure.Interfaces;
using System.Security.Claims;

namespace SGCP.Application.Services.ModuloUsuarios
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IJwtTokenService _jwtTokenService;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IJwtTokenService jwtTokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtTokenService = jwtTokenService;
        }

        public int? GetUserId()
        {
            var userIdClaim = GetClaimValue("UserId") ?? GetClaimValue(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim, out int userId))
                return userId;

            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var userIdString = session.GetString("UserId");
                if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int sessionUserId))
                    return sessionUserId;
            }

            return null;
        }

        public string GetUserName()
        {
            var username = GetClaimValue(ClaimTypes.Name) ?? GetClaimValue("Username");
            if (!string.IsNullOrEmpty(username))
                return username;

            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var sessionUsername = session.GetString("Username");
                if (!string.IsNullOrEmpty(sessionUsername))
                    return sessionUsername;
            }

            return string.Empty;
        }

        private string? GetClaimValue(string claimType)
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(claimType);
            if (claim != null)
                return claim.Value;

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
