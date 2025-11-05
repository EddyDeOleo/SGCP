using Microsoft.Extensions.Logging;
using Moq;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Test.Application.ModuloUsuarios;


public class ClienteServiceTest
{
    private readonly Mock<ICliente> _repoMock;
    private readonly Mock<ILogger<ClienteService>> _loggerMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly ClienteService _service;

    public ClienteServiceTest()
    {
        _repoMock = new Mock<ICliente>();
        _loggerMock = new Mock<ILogger<ClienteService>>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(s => s.GetUserId()).Returns(1);
        _service = new ClienteService(_repoMock.Object, _loggerMock.Object, _currentUserMock.Object);
    }

    #region CreateCliente Tests

    [Fact]
    public async Task CreateCliente_ShouldFail_WhenUsernameExists()
    {
        var existingCliente = new Cliente("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetAll())
                 .ReturnsAsync(OperationResult.SuccessResult("", new List<Cliente> { existingCliente }));

        var dto = new CreateClienteDTO { Nombre = "Carlos", Apellido = "Gomez", Username = "jperez", Password = "pass" };
        var result = await _service.CreateCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("El username ya está registrado", result.Message);
    }

    [Fact]
    public async Task CreateCliente_ShouldFail_WhenRepositorySaveFails()
    {
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(OperationResult.SuccessResult("", new List<Cliente>()));
        _repoMock.Setup(r => r.Save(It.IsAny<Cliente>()))
                 .ReturnsAsync(OperationResult.FailureResult("Error al guardar"));

        var dto = new CreateClienteDTO { Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "pass" };
        var result = await _service.CreateCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("Error al guardar", result.Message);
    }

    [Fact]
    public async Task CreateCliente_ShouldSucceed_WhenDataIsValid()
    {
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(OperationResult.SuccessResult("", new List<Cliente>()));
        _repoMock.Setup(r => r.Save(It.IsAny<Cliente>()))
                 .ReturnsAsync(OperationResult.SuccessResult("Guardado", null));

        var dto = new CreateClienteDTO { Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "pass" };
        var result = await _service.CreateCliente(dto);

        Assert.True(result.Success);
        Assert.Equal("Cliente creado correctamente", result.Message);
    }

    [Fact]
    public async Task CreateCliente_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetAll()).ThrowsAsync(new Exception("DB error"));

        var dto = new CreateClienteDTO { Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "pass" };
        var result = await _service.CreateCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al crear el cliente", result.Message);
    }

    #endregion

    #region GetCliente Tests

    [Fact]
    public async Task GetCliente_ShouldReturnClientes_WhenSuccess()
    {
        var clientes = new List<Cliente> { new Cliente("Juan", "Perez", "jperez", "pass") };
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(OperationResult.SuccessResult("", clientes));

        var result = await _service.GetCliente();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        var dtos = (List<ClienteGetDTO>)result.Data;
        Assert.Single(dtos);
        Assert.Equal("jperez", dtos[0].Username);
    }

    [Fact]
    public async Task GetCliente_ShouldFail_WhenRepositoryFails()
    {
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(OperationResult.FailureResult("DB error"));

        var result = await _service.GetCliente();

        Assert.False(result.Success);
        Assert.Equal("No se pudieron obtener los clientes", result.Message);
    }

    [Fact]
    public async Task GetCliente_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetAll()).ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetCliente();

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al obtener los clientes", result.Message);
    }

    #endregion

    #region GetClienteById Tests

    [Fact]
    public async Task GetClienteById_ShouldReturnCliente_WhenExists()
    {
        var cliente = new Cliente("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", cliente));

        var result = await _service.GetClienteById(1);

        Assert.True(result.Success);
        Assert.Equal("jperez", ((ClienteGetDTO)result.Data).Username);
    }

    [Fact]
    public async Task GetClienteById_ShouldFail_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", null));

        var result = await _service.GetClienteById(1);

        Assert.False(result.Success);
        Assert.Equal("Cliente no encontrado", result.Message);
    }

    [Fact]
    public async Task GetClienteById_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetClienteById(1);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al obtener el cliente", result.Message);
    }

    #endregion

    #region UpdateCliente Tests

    [Fact]
    public async Task UpdateCliente_ShouldFail_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", null));

        var dto = new UpdateClienteDTO { ClienteId = 1, Nombre = "A", Apellido = "B", Username = "u", Password = "p" };
        var result = await _service.UpdateCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("Cliente no encontrado", result.Message);
    }

    [Fact]
    public async Task UpdateCliente_ShouldFail_WhenUpdateFails()
    {
        var cliente = new Cliente("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", cliente));
        _repoMock.Setup(r => r.Update(It.IsAny<Cliente>())).ReturnsAsync(OperationResult.FailureResult("Update error"));

        var dto = new UpdateClienteDTO { ClienteId = 1, Nombre = "A", Apellido = "B", Username = "u", Password = "p" };
        var result = await _service.UpdateCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("Update error", result.Message);
    }

    [Fact]
    public async Task UpdateCliente_ShouldSucceed_WhenValid()
    {
        var cliente = new Cliente("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", cliente));
        _repoMock.Setup(r => r.Update(It.IsAny<Cliente>())).ReturnsAsync(OperationResult.SuccessResult("Updated", cliente));

        var dto = new UpdateClienteDTO { ClienteId = 1, Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "newpass" };
        var result = await _service.UpdateCliente(dto);

        Assert.True(result.Success);
        Assert.Equal("Cliente actualizado correctamente", result.Message);
    }

    [Fact]
    public async Task UpdateCliente_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ThrowsAsync(new Exception("DB error"));

        var dto = new UpdateClienteDTO { ClienteId = 1, Nombre = "A", Apellido = "B", Username = "u", Password = "p" };
        var result = await _service.UpdateCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al actualizar el cliente", result.Message);
    }

    #endregion

    #region RemoveCliente Tests

    [Fact]
    public async Task RemoveCliente_ShouldFail_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", null));

        var dto = new DeleteClienteDTO { ClienteId = 1 };
        var result = await _service.RemoveCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("Cliente no encontrado", result.Message);
    }

    [Fact]
    public async Task RemoveCliente_ShouldFail_WhenRemoveFails()
    {
        var cliente = new Cliente("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", cliente));
        _repoMock.Setup(r => r.Remove(cliente)).ReturnsAsync(OperationResult.FailureResult("Remove error"));

        var dto = new DeleteClienteDTO { ClienteId = 1 };
        var result = await _service.RemoveCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("Remove error", result.Message);
    }

    [Fact]
    public async Task RemoveCliente_ShouldSucceed_WhenValid()
    {
        var cliente = new Cliente("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", cliente));
        _repoMock.Setup(r => r.Remove(cliente)).ReturnsAsync(OperationResult.SuccessResult("Removed", null));

        var dto = new DeleteClienteDTO { ClienteId = 1 };
        var result = await _service.RemoveCliente(dto);

        Assert.True(result.Success);
        Assert.Equal("Cliente eliminado correctamente", result.Message);
    }

    [Fact]
    public async Task RemoveCliente_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ThrowsAsync(new Exception("DB error"));

        var dto = new DeleteClienteDTO { ClienteId = 1 };
        var result = await _service.RemoveCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al eliminar el cliente", result.Message);
    }

    #endregion
}