using Microsoft.Extensions.Logging;
using Moq;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;
using SGCP.Application.Interfaces;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Application.Test;

public class AdminServiceTests
{
    private readonly Mock<IAdministrador> _repoMock;
    private readonly Mock<ILogger<AdminService>> _loggerMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly AdminService _service;

    public AdminServiceTests()
    {
        _repoMock = new Mock<IAdministrador>();
        _loggerMock = new Mock<ILogger<AdminService>>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.GetUserId()).Returns(1);

        _service = new AdminService(_repoMock.Object, _loggerMock.Object, _currentUserMock.Object);
    }

    #region CreateAdmin Tests

    [Fact]
    public async Task CreateAdmin_ShouldFail_WhenUsernameExists()
    {
        var existingAdmin = new Administrador("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetAll())
                 .ReturnsAsync(OperationResult.SuccessResult("", new List<Administrador> { existingAdmin }));

        var dto = new CreateAdminDTO { Nombre = "Carlos", Apellido = "Gomez", Username = "jperez", Password = "pass" };
        var result = await _service.CreateAdmin(dto);

        Assert.False(result.Success);
        Assert.Equal("El username ya está registrado", result.Message);
    }

    [Fact]
    public async Task CreateAdmin_ShouldFail_WhenRepositorySaveFails()
    {
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(OperationResult.SuccessResult("", new List<Administrador>()));
        _repoMock.Setup(r => r.Save(It.IsAny<Administrador>()))
                 .ReturnsAsync(OperationResult.FailureResult("Error al guardar"));

        var dto = new CreateAdminDTO { Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "pass" };
        var result = await _service.CreateAdmin(dto);

        Assert.False(result.Success);
        Assert.Equal("Error al guardar", result.Message);
    }

    [Fact]
    public async Task CreateAdmin_ShouldSucceed_WhenDataIsValid()
    {
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(OperationResult.SuccessResult("", new List<Administrador>()));
        _repoMock.Setup(r => r.Save(It.IsAny<Administrador>()))
                 .ReturnsAsync(OperationResult.SuccessResult("Guardado", null));

        var dto = new CreateAdminDTO { Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "pass" };
        var result = await _service.CreateAdmin(dto);

        Assert.True(result.Success);
        Assert.Equal("Administrador creado correctamente", result.Message);
    }

    [Fact]
    public async Task CreateAdmin_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetAll()).ThrowsAsync(new Exception("DB error"));

        var dto = new CreateAdminDTO { Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "pass" };
        var result = await _service.CreateAdmin(dto);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al crear el administrador", result.Message);
    }

    #endregion

    #region GetAdmin Tests

    [Fact]
    public async Task GetAdmin_ShouldReturnAdmins_WhenSuccess()
    {
        var admins = new List<Administrador>
        {
            new Administrador("Juan","Perez","jperez","pass")
        };
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(OperationResult.SuccessResult("", admins));

        var result = await _service.GetAdmin();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        var dtos = (List<AdminGetDTO>)result.Data;
        Assert.Single(dtos);
        Assert.Equal("jperez", dtos[0].Username);
    }

    [Fact]
    public async Task GetAdmin_ShouldFail_WhenRepositoryFails()
    {
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync(OperationResult.FailureResult("DB error"));

        var result = await _service.GetAdmin();

        Assert.False(result.Success);
        Assert.Equal("No se pudieron obtener los administradores", result.Message);
    }

    [Fact]
    public async Task GetAdmin_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetAll()).ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetAdmin();

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al obtener los administradores", result.Message);
    }

    #endregion

    #region GetAdminById Tests

    [Fact]
    public async Task GetAdminById_ShouldReturnAdmin_WhenExists()
    {
        var admin = new Administrador("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", admin));

        var result = await _service.GetAdminById(1);

        Assert.True(result.Success);
        Assert.Equal("jperez", ((AdminGetDTO)result.Data).Username);
    }

    [Fact]
    public async Task GetAdminById_ShouldFail_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", null));

        var result = await _service.GetAdminById(1);

        Assert.False(result.Success);
        Assert.Equal("Administrador no encontrado", result.Message);
    }

    [Fact]
    public async Task GetAdminById_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ThrowsAsync(new Exception("DB error"));

        var result = await _service.GetAdminById(1);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al obtener el administrador", result.Message);
    }

    #endregion

    #region UpdateAdmin Tests

    [Fact]
    public async Task UpdateAdmin_ShouldFail_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", null));

        var dto = new UpdateAdminDTO { AdminId = 1, Nombre = "A", Apellido = "B", Username = "u", Password = "p" };
        var result = await _service.UpdateAdmin(dto);

        Assert.False(result.Success);
        Assert.Equal("Administrador no encontrado", result.Message);
    }

    [Fact]
    public async Task UpdateAdmin_ShouldFail_WhenUpdateFails()
    {
        var admin = new Administrador("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", admin));
        _repoMock.Setup(r => r.Update(It.IsAny<Administrador>())).ReturnsAsync(OperationResult.FailureResult("Update error"));

        var dto = new UpdateAdminDTO { AdminId = 1, Nombre = "A", Apellido = "B", Username = "u", Password = "p" };
        var result = await _service.UpdateAdmin(dto);

        Assert.False(result.Success);
        Assert.Equal("Update error", result.Message);
    }

    [Fact]
    public async Task UpdateAdmin_ShouldSucceed_WhenValid()
    {
        var admin = new Administrador("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", admin));
        _repoMock.Setup(r => r.Update(It.IsAny<Administrador>())).ReturnsAsync(OperationResult.SuccessResult("Updated", admin));

        var dto = new UpdateAdminDTO { AdminId = 1, Nombre = "Carlos", Apellido = "Gomez", Username = "cgomez", Password = "newpass" };
        var result = await _service.UpdateAdmin(dto);

        Assert.True(result.Success);
        Assert.Equal("Administrador actualizado correctamente", result.Message);
    }

    [Fact]
    public async Task UpdateAdmin_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ThrowsAsync(new Exception("DB error"));

        var dto = new UpdateAdminDTO { AdminId = 1, Nombre = "A", Apellido = "B", Username = "u", Password = "p" };
        var result = await _service.UpdateAdmin(dto);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al actualizar el administrador", result.Message);
    }

    #endregion

    #region RemoveAdmin Tests

    [Fact]
    public async Task RemoveAdmin_ShouldFail_WhenNotFound()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", null));

        var dto = new DeleteAdminDTO { AdminId = 1 };
        var result = await _service.RemoveAdmin(dto);

        Assert.False(result.Success);
        Assert.Equal("Administrador no encontrado", result.Message);
    }

    [Fact]
    public async Task RemoveAdmin_ShouldFail_WhenRemoveFails()
    {
        var admin = new Administrador("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", admin));
        _repoMock.Setup(r => r.Remove(admin)).ReturnsAsync(OperationResult.FailureResult("Remove error"));

        var dto = new DeleteAdminDTO { AdminId = 1 };
        var result = await _service.RemoveAdmin(dto);

        Assert.False(result.Success);
        Assert.Equal("Remove error", result.Message);
    }

    [Fact]
    public async Task RemoveAdmin_ShouldSucceed_WhenValid()
    {
        var admin = new Administrador("Juan", "Perez", "jperez", "pass");
        _repoMock.Setup(r => r.GetEntityBy(1)).ReturnsAsync(OperationResult.SuccessResult("", admin));
        _repoMock.Setup(r => r.Remove(admin)).ReturnsAsync(OperationResult.SuccessResult("Removed", null));

        var dto = new DeleteAdminDTO { AdminId = 1 };
        var result = await _service.RemoveAdmin(dto);

        Assert.True(result.Success);
        Assert.Equal("Administrador eliminado correctamente", result.Message);
    }

    [Fact]
    public async Task RemoveAdmin_ShouldHandleException()
    {
        _repoMock.Setup(r => r.GetEntityBy(1)).ThrowsAsync(new Exception("DB error"));

        var dto = new DeleteAdminDTO { AdminId = 1 };
        var result = await _service.RemoveAdmin(dto);

        Assert.False(result.Success);
        Assert.Equal("Ocurrió un error al eliminar el administrador", result.Message);
    }

    #endregion
}