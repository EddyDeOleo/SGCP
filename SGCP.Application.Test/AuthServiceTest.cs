using Microsoft.Extensions.Logging;
using Moq;
using SGCP.Application.Dtos.ModuloUsuarios.Authetication;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Infraestructure.Interfaces;

namespace SGCP.Test.Application.ModuloUsuarios;


public class AuthServiceTests
{
    private readonly Mock<IAdministrador> _adminRepoMock;
    private readonly Mock<ICliente> _clienteRepoMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock; 
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _adminRepoMock = new Mock<IAdministrador>();
        _clienteRepoMock = new Mock<ICliente>();
        _loggerMock = new Mock<ILogger<AuthService>>();
        _jwtServiceMock = new Mock<IJwtTokenService>(); 

        _service = new AuthService(
            _adminRepoMock.Object,
            _clienteRepoMock.Object,
            _loggerMock.Object,
            _jwtServiceMock.Object 
        );
    }

    [Fact]
    public async Task Login_ShouldFail_WhenUsernameOrPasswordIsEmpty()
    {
        var dto = new LoginDTO { Username = "", Password = "" };

        var result = await _service.Login(dto);

        Assert.False(result.Success);
        Assert.Equal("Usuario y contraseña son requeridos", result.Message);
    }

    [Fact]
    public async Task Login_ShouldFail_WhenCredentialsAreInvalid()
    {
        var dto = new LoginDTO { Username = "user1", Password = "wrong" };

        _adminRepoMock.Setup(r => r.GetAll())
            .ReturnsAsync(OperationResult.SuccessResult("OK", new List<Administrador>()));
        _clienteRepoMock.Setup(r => r.GetAll())
            .ReturnsAsync(OperationResult.SuccessResult("OK", new List<Cliente>()));

        var result = await _service.Login(dto);

        Assert.False(result.Success);
        Assert.Equal("Credenciales inválidas", result.Message);
    }

    [Fact]
    public async Task Login_ShouldSucceed_ForValidAdmin()
    {
        var dto = new LoginDTO { Username = "admin1", Password = "pass" };
        var admin = new Administrador("Admin", "Uno", "admin1", "pass") { IdUsuario = 1, Estatus = true };

        _adminRepoMock.Setup(r => r.GetAll())
            .ReturnsAsync(OperationResult.SuccessResult("OK", new List<Administrador> { admin }));

        _jwtServiceMock.Setup(j => j.GenerateToken(admin.IdUsuario, admin.Username, admin.Nombre, admin.Apellido))
            .Returns("token123");

        var result = await _service.Login(dto);

        Assert.True(result.Success);
        var data = Assert.IsType<AuthResponseDTO>(result.Data);
        Assert.Equal("token123", data.Token);
        Assert.Equal(admin.IdUsuario, data.UserId);
    }

    [Fact]
    public async Task Login_ShouldSucceed_ForValidCliente()
    {
        var dto = new LoginDTO { Username = "client1", Password = "pass" };
        var cliente = new Cliente("Cliente", "Uno", "client1", "pass") { IdUsuario = 2, Estatus = true };

        _adminRepoMock.Setup(r => r.GetAll())
            .ReturnsAsync(OperationResult.SuccessResult("OK", new List<Administrador>()));
        _clienteRepoMock.Setup(r => r.GetAll())
            .ReturnsAsync(OperationResult.SuccessResult("OK", new List<Cliente> { cliente }));

        _jwtServiceMock.Setup(j => j.GenerateToken(cliente.IdUsuario, cliente.Username, cliente.Nombre, cliente.Apellido))
            .Returns("token456");

        var result = await _service.Login(dto);

        Assert.True(result.Success);
        var data = Assert.IsType<AuthResponseDTO>(result.Data);
        Assert.Equal("token456", data.Token);
        Assert.Equal(cliente.IdUsuario, data.UserId);
    }

    [Fact]
    public async Task Logout_ShouldReturnSuccess()
    {
        var result = await _service.Logout(1);

        Assert.True(result.Success);
        Assert.Equal("Logout exitoso", result.Message);
    }
}