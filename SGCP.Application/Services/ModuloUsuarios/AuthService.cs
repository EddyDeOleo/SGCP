

using Microsoft.Extensions.Logging;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Authetication;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Infraestructure.Interfaces;
using SGCP.Infraestructure.Security;


namespace SGCP.Application.Services.ModuloUsuarios
{
    public sealed class AuthService : IAuthService
    {
        private readonly IAdministrador _adminRepository;
        private readonly ICliente _clienteRepository;
        private readonly ILogger<AuthService> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IAdministrador adminRepository,
            ICliente clienteRepository,
            ILogger<AuthService> logger,
            IJwtTokenService jwtTokenService)
        {
            _adminRepository = adminRepository;
            _clienteRepository = clienteRepository;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }
        public async Task<ServiceResult> Login(LoginDTO loginDto)
        {
            var result = new ServiceResult();
            _logger.LogInformation("Iniciando login para usuario: {Username}", loginDto.Username);

            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                result.Success = false;
                result.Message = "Usuario y contraseña son requeridos";
                return result;
            }

            try
            {
                var admin = await BuscarAdministrador(loginDto.Username, loginDto.Password);
                if (admin != null)
                {
                    var token = _jwtTokenService.GenerateToken(admin.IdUsuario, admin.Username, admin.Nombre, admin.Apellido);
                    return CrearResultadoExitoso(admin.IdUsuario, admin.Username, admin.Nombre, admin.Apellido, token);
                }

                var cliente = await BuscarCliente(loginDto.Username, loginDto.Password);
                if (cliente != null)
                {
                    var token = _jwtTokenService.GenerateToken(cliente.IdUsuario, cliente.Username, cliente.Nombre, cliente.Apellido);
                    return CrearResultadoExitoso(cliente.IdUsuario, cliente.Username, cliente.Nombre, cliente.Apellido, token);
                }

                _logger.LogWarning("Intento de login fallido para {Username}", loginDto.Username);

                result.Success = false;
                result.Message = "Credenciales inválidas";
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el login");

                result.Success = false;
                result.Message = "Ocurrió un error durante el login";
                return result;
            }
        }

        private ServiceResult CrearResultadoExitoso(int id, string username, string nombre, string apellido, string token)
        {
            _logger.LogInformation("Login exitoso para usuario {UserId}", id);

            return new ServiceResult
            {
                Success = true,
                Message = "Login exitoso",
                Data = new AuthResponseDTO
                {
                    UserId = id,
                    Username = username,
                    Nombre = nombre,
                    Apellido = apellido,
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(8)
                }
            };
        }

        private async Task<Administrador?> BuscarAdministrador(string username, string password)
        {
            var usuarios = await _adminRepository.GetAll();
            if (!usuarios.Success || usuarios.Data == null) return null;

            var admin = ((List<Administrador>)usuarios.Data)
                .FirstOrDefault(a => a.Username == username && a.Estatus);

            return admin != null && VerifyPassword(password, admin.Password) ? admin : null;
        }

        private async Task<Cliente?> BuscarCliente(string username, string password)
        {
            var clientes = await _clienteRepository.GetAll();
            if (!clientes.Success || clientes.Data == null) return null;

            var cliente = ((List<Cliente>)clientes.Data)
                .FirstOrDefault(c => c.Username == username && c.Estatus);

            return cliente != null && VerifyPassword(password, cliente.Password) ? cliente : null;
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
       
            return inputPassword == storedPassword;
        }

        public async Task<ServiceResult> Logout(int userId)
        {
            _logger.LogInformation("Usuario {UserId} cerró sesión", userId);

            return await Task.FromResult(new ServiceResult
            {
                Success = true,
                Message = "Logout exitoso"
            });
        }
    }
}
