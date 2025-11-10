using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Dtos.ModuloUsuarios.Authetication;
using SGCP.Application.Interfaces.ModuloUsuarios;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGCP.ModuloUsuarios.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            var result = await _authService.Login(loginDto);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("logout-user")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var result = await _authService.Logout(userId);
                return Ok(result);
            }

            return Ok(new { message = "Sesión cerrada" });
        }

    
        [HttpGet("test-user")]
        [Authorize]

        public IActionResult TestUser([FromServices] ICurrentUserService currentUser)
        {
            return Ok(new
            {
                UserId = currentUser.GetUserId(),
                Username = currentUser.GetUserName()
            });
        }
    }
}
