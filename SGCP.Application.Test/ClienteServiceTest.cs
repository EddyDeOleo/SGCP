using Microsoft.Extensions.Logging;
using Moq;
using SGCP.Application.Base;
using SGCP.Application.Base.ServiceValidator.ModuloUsuarios;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Application.Interfaces.IServiceValidator.ModuloUsuarios;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Test.Application.ModuloUsuarios;


public class ClienteServiceTest
{
    private readonly Mock<ICliente> _repoMock;
    private readonly Mock<ILogger<ClienteService>> _loggerMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IClienteServiceValidator> _validatorMock;
    private readonly ClienteService _service;

    public ClienteServiceTest()
    {
        _repoMock = new Mock<ICliente>();
        _loggerMock = new Mock<ILogger<ClienteService>>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(s => s.GetUserId()).Returns(1);

        _validatorMock = new Mock<IClienteServiceValidator>();

        _service = new ClienteService(
            _repoMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object,
            _validatorMock.Object
        );
    }

    #region CreateCliente Tests
    [Fact]
    public async Task CreateCliente_ShouldFail_WhenUsernameExists()
    {
        // Arrange
        var dto = new CreateClienteDTO
        {
            Username = "user1",
            Nombre = "Juan",
            Apellido = "Pérez",
            Password = "1234"
        };

        var repoMock = new Mock<ICliente>();
        var loggerMock = new Mock<ILogger<ClienteService>>();
        var currentUserMock = new Mock<ICurrentUserService>();
        var validatorMock = new Mock<IClienteServiceValidator>();

        repoMock.Setup(r => r.Save(It.IsAny<Domain.Entities.ModuloDeUsuarios.Cliente>()))
                .ReturnsAsync(OperationResult.SuccessResult("", null));

        validatorMock.Setup(v => v.ValidateForCreate(It.IsAny<CreateClienteDTO>()))
                     .Returns(new ServiceResult(true, "DTO válido"));

        validatorMock.Setup(v => v.ValidateUsernameUnico(It.IsAny<string>()))
                     .ReturnsAsync(new ServiceResult(false, "El username ya está registrado"));

        var service = new ClienteService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await service.CreateCliente(dto);

        // Assert
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
        Assert.Equal("Ocurrió un error al crear cliente", result.Message);
    }

    [Fact]
    public async Task CreateCliente_ShouldSucceed_WhenDataIsValid()
    {
        // Arrange
        var dto = new CreateClienteDTO
        {
            Nombre = "Carlos",
            Apellido = "Gomez",
            Username = "cgomez",
            Password = "pass"
        };

        // Mock del validador: el username es válido y único
        _validatorMock.Setup(v => v.ValidateUsernameUnico(dto.Username))
            .ReturnsAsync(new ServiceResult(true, "Username válido y único"));

        // Mock del validador de DTO
        _validatorMock.Setup(v => v.ValidateForCreate(dto))
            .Returns(new ServiceResult(true, "DTO válido"));

        // Mock del repositorio
        _repoMock.Setup(r => r.GetAll())
            .ReturnsAsync(OperationResult.SuccessResult("", new List<Cliente>()));

        _repoMock.Setup(r => r.Save(It.IsAny<Cliente>()))
            .ReturnsAsync(OperationResult.SuccessResult("Guardado", null));

        _currentUserMock.Setup(s => s.GetUserId()).Returns(1);

        // Act
        var result = await _service.CreateCliente(dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success, $"Falló con el mensaje: {result.Message}");
        Assert.Equal("Cliente creado correctamente", result.Message);
    }


    [Fact]
    public async Task CreateCliente_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetAll()).ThrowsAsync(new Exception("DB error"));

        var dto = new CreateClienteDTO { Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "pass" };
        var result = await _service.CreateCliente(dto);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al crear cliente", result.Message);

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
        Assert.Equal("Ocurrió un error al obtener todos los clientes", result.Message);
    }

    #endregion

    #region GetClienteById Tests
    [Fact]
    public async Task GetClienteById_ShouldReturnCliente_WhenExists()
    {
        // Arrange
        var cliente = new Cliente
        {
            IdUsuario = 1,
            Nombre = "Carlos",
            Apellido = "Gomez",
            Username = "cgomez"
        };

        _validatorMock.Setup(v => v.ValidateClienteExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(true, "Cliente existente", cliente));

        var result = await _service.GetClienteById(1);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Cliente obtenido correctamente", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetClienteById_ShouldFail_WhenNotFound()
    {
        // Arrange
        _validatorMock.Setup(v => v.ValidateClienteExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(false, "Cliente no encontrado"));

        var result = await _service.GetClienteById(99);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Cliente no encontrado", result.Message);
    }

    [Fact]
    public async Task GetClienteById_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetClienteById(1);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al obtener cliente", result.Message);
    }

    #endregion

    #region UpdateCliente Tests

    [Fact]
    public async Task UpdateCliente_ShouldFail_WhenNotFound()
    {
        // Arrange
        var dto = new UpdateClienteDTO { ClienteId = 1, Nombre = "A", Apellido = "B", Username = "u", Password = "p" };

        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", null));

        _validatorMock.Setup(v => v.ValidateForUpdate(It.IsAny<UpdateClienteDTO>()))
                      .Returns(new ServiceResult(true, "DTO válido"));

        // Act
        var result = await _service.UpdateCliente(dto);

        // Assert
        Assert.False(result.Success);
        Assert.True(
            result.Message != null &&
            (result.Message.Contains("Cliente no encontrado") || result.Message.Contains("actualizar cliente")),
            $"Mensaje inesperado: {result.Message}"
        );
    }

    [Fact]
    public async Task UpdateCliente_ShouldFail_WhenUpdateFails()
    {
        // Arrange
        var cliente = new Cliente("Juan", "Perez", "jperez", "pass");
        var dto = new UpdateClienteDTO { ClienteId = 1, Nombre = "A", Apellido = "B", Username = "u", Password = "p" };

        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", cliente));

        _validatorMock.Setup(v => v.ValidateForUpdate(It.IsAny<UpdateClienteDTO>()))
                      .Returns(new ServiceResult(true, "DTO válido"));
        _validatorMock.Setup(v => v.ValidateClienteExistente(1))
                      .ReturnsAsync(new ServiceResult(true, "Cliente existente", cliente));

        _repoMock.Setup(r => r.Update(It.IsAny<Cliente>()))
                 .ReturnsAsync(OperationResult.FailureResult("Update error"));

        // Act
        var result = await _service.UpdateCliente(dto);

        // Assert
        Assert.False(result.Success);
        Assert.True(
            result.Message != null &&
            (result.Message.Contains("Update error") || result.Message.Contains("actualizar cliente")),
            $"Mensaje inesperado: {result.Message}"
        );
    }

    [Fact]
    public async Task UpdateCliente_ShouldSucceed_WhenValid()
    {
        // Arrange
        var cliente = new Cliente("Juan", "Perez", "jperez", "pass");
        var dto = new UpdateClienteDTO { ClienteId = 1, Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "newpass" };

        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", cliente));

        _validatorMock.Setup(v => v.ValidateForUpdate(It.IsAny<UpdateClienteDTO>()))
                      .Returns(new ServiceResult(true, "DTO válido"));
        _validatorMock.Setup(v => v.ValidateClienteExistente(1))
                      .ReturnsAsync(new ServiceResult(true, "Cliente existente", cliente));

        _repoMock.Setup(r => r.Update(It.IsAny<Cliente>()))
                 .ReturnsAsync(OperationResult.SuccessResult("Updated", cliente));

        // Act
        var result = await _service.UpdateCliente(dto);

        // Assert
        Assert.True(result.Success, $"Felló con mensaje: {result.Message}");
        Assert.Equal("Cliente actualizado correctamente", result.Message);
    }

    [Fact]
    public async Task UpdateCliente_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ThrowsAsync(new Exception("DB error"));

        var dto = new UpdateClienteDTO { ClienteId = 1, Nombre = "A", Apellido = "B", Username = "u", Password = "p" };
        var result = await _service.UpdateCliente(dto);

        Assert.False(result.Success);
        Assert.Equal($"Ocurrió un error al actualizar cliente", result.Message);
    }

    #endregion

    #region RemoveCliente Tests

    [Fact]
    public async Task RemoveCliente_ShouldFail_WhenNotFound()
    {
        // Arrange
        var dto = new DeleteClienteDTO { ClienteId = 1 };

        _validatorMock.Setup(v => v.ValidateForDelete(It.IsAny<DeleteClienteDTO>()))
            .Returns(new ServiceResult(true, "DTO válido"));

        _validatorMock.Setup(v => v.ValidateClienteExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(false, "Cliente no encontrado"));

        var result = await _service.RemoveCliente(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Cliente no encontrado", result.Message);
    }

    [Fact]
    public async Task RemoveCliente_ShouldFail_WhenRemoveFails()
    {
        // Arrange
        var dto = new DeleteClienteDTO { ClienteId = 1 };
        var cliente = new Cliente { IdUsuario = 1, Nombre = "Carlos" };

        _validatorMock.Setup(v => v.ValidateForDelete(It.IsAny<DeleteClienteDTO>()))
            .Returns(new ServiceResult(true, "DTO válido"));

        _validatorMock.Setup(v => v.ValidateClienteExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(true, "Cliente existente", cliente));

        _repoMock.Setup(r => r.Remove(It.IsAny<Cliente>()))
            .ReturnsAsync(OperationResult.FailureResult("Remove error"));

        var result = await _service.RemoveCliente(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Remove error", result.Message);
    }

    [Fact]
    public async Task RemoveCliente_ShouldSucceed_WhenValid()
    {
        // Arrange
        var dto = new DeleteClienteDTO { ClienteId = 1 };
        var cliente = new Cliente { IdUsuario = 1, Nombre = "Carlos" };

        _validatorMock.Setup(v => v.ValidateForDelete(It.IsAny<DeleteClienteDTO>()))
            .Returns(new ServiceResult(true, "DTO válido"));

        _validatorMock.Setup(v => v.ValidateClienteExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(true, "Cliente existente", cliente));

        _repoMock.Setup(r => r.Remove(It.IsAny<Cliente>()))
            .ReturnsAsync(OperationResult.SuccessResult("Cliente eliminado"));

        var result = await _service.RemoveCliente(dto);

        // Assert
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
        Assert.Equal("Ocurrió un error al eliminar cliente", result.Message);
    }

    #endregion
}