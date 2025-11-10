using Microsoft.Extensions.Logging;
using Moq;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;
using SGCP.Application.Interfaces.IServiceValidator.ModuloUsuarios;
using SGCP.Application.Interfaces.ModuloUsuarios;
using SGCP.Application.Repositories.ModuloUsuarios;
using SGCP.Application.Services.ModuloUsuarios;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeUsuarios;

namespace SGCP.Test.Application.ModuloUsuarios;


public class AdminServiceTests
{
    private readonly Mock<IAdministrador> _repoMock;
    private readonly Mock<ILogger<AdminService>> _loggerMock;
    private readonly Mock<ICurrentUserService> _currentUserMock;
    private readonly Mock<IAdminServiceValidator> _validatorMock;
    private readonly AdminService _service;

    public AdminServiceTests()
    {
        _repoMock = new Mock<IAdministrador>();
        _loggerMock = new Mock<ILogger<AdminService>>();
        _currentUserMock = new Mock<ICurrentUserService>();
        _currentUserMock.Setup(c => c.GetUserId()).Returns(1);

        // Mock del validator
        _validatorMock = new Mock<IAdminServiceValidator>();

        // Inyectamos mocks en el servicio
        _service = new AdminService(
            _repoMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object,
            _validatorMock.Object
        );
    }



    #region CreateAdmin Tests

    [Fact]
    public async Task CreateAdmin_ShouldFail_WhenUsernameExists()
    {
        // Arrange
        var dto = new CreateAdminDTO
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Username = "jperez",
            Password = "1234"
        };

        var repoMock = new Mock<IAdministrador>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();
        var validatorMock = new Mock<IAdminServiceValidator>();

        validatorMock.Setup(v => v.ValidateForCreate(It.IsAny<CreateAdminDTO>()))
            .Returns(new ServiceResult(true, "DTO válido"));

        validatorMock.Setup(v => v.ValidateUsernameUnico(It.IsAny<string>()))
            .ReturnsAsync(new ServiceResult(false, "El username ya está registrado"));

        var service = new AdminService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await service.CreateAdmin(dto);

        // Assert
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
        Assert.Equal("Ocurrió un error al crear el administrador", result.Message);
    }

    [Fact]
    public async Task CreateAdmin_ShouldSucceed_WhenDataIsValid()
    {
        // Arrange
        var dto = new CreateAdminDTO
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Username = "juanp",
            Password = "12345"
        };

        var repoMock = new Mock<IAdministrador>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();
        var validatorMock = new Mock<IAdminServiceValidator>();

        validatorMock.Setup(v => v.ValidateForCreate(dto))
            .Returns(new ServiceResult(true, "DTO válido"));

        validatorMock.Setup(v => v.ValidateUsernameUnico(dto.Username))
            .ReturnsAsync(new ServiceResult(true, "Username válido y único"));

        repoMock.Setup(r => r.Save(It.IsAny<Administrador>()))
            .ReturnsAsync(OperationResult.SuccessResult("Administrador guardado correctamente"));

        var service = new AdminService(repoMock.Object, loggerMock.Object, currentUserMock.Object, validatorMock.Object);

        // Act
        var result = await service.CreateAdmin(dto);

        // Assert
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
        // Arrange
        var repoMock = new Mock<IAdministrador>();
        var validatorMock = new Mock<IAdminServiceValidator>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();

        var admin = new Administrador("Juan", "Pérez", "juanp", "1234");

        validatorMock.Setup(v => v.ValidateAdminExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(true, "Administrador existente", admin));

        var service = new AdminService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await service.GetAdminById(1);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Administrador obtenido correctamente", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetAdminById_ShouldFail_WhenNotFound()
    {
        // Arrange
        var repoMock = new Mock<IAdministrador>();
        var validatorMock = new Mock<IAdminServiceValidator>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();

        validatorMock.Setup(v => v.ValidateAdminExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(false, "Administrador no encontrado"));

        var service = new AdminService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await service.GetAdminById(1);

        // Assert
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
        // Arrange
        var repoMock = new Mock<IAdministrador>();
        var validatorMock = new Mock<IAdminServiceValidator>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();

        validatorMock.Setup(v => v.ValidateForUpdate(It.IsAny<UpdateAdminDTO>()))
            .Returns(new ServiceResult(true, "DTO válido para actualizar administrador"));

        validatorMock.Setup(v => v.ValidateAdminExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(false, "Administrador no encontrado"));

        var service = new AdminService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        var dto = new UpdateAdminDTO
        {
            AdminId = 1,
            Nombre = "Juan",
            Apellido = "Pérez",
            Username = "jperez"
        };

        // Act
        var result = await service.UpdateAdmin(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Administrador no encontrado", result.Message);
    }

    [Fact]
    public async Task UpdateAdmin_ShouldFail_WhenUpdateFails()
    {
        // Arrange
        var repoMock = new Mock<IAdministrador>();
        var validatorMock = new Mock<IAdminServiceValidator>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();

        var admin = new Administrador("Juan", "Pérez", "jperez", "1234");

        validatorMock.Setup(v => v.ValidateForUpdate(It.IsAny<UpdateAdminDTO>()))
            .Returns(new ServiceResult(true, "DTO válido para actualizar administrador"));

        validatorMock.Setup(v => v.ValidateAdminExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(true, "Administrador existente", admin));

        repoMock.Setup(r => r.Update(It.IsAny<Administrador>()))
            .ReturnsAsync(OperationResult.FailureResult("Update error"));

        var service = new AdminService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        var dto = new UpdateAdminDTO
        {
            AdminId = 1,
            Nombre = "Juan",
            Apellido = "Pérez",
            Username = "jperez"
        };

        // Act
        var result = await service.UpdateAdmin(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Update error", result.Message);
    }

    [Fact]
    public async Task UpdateAdmin_ShouldSucceed_WhenValid()
    {
        // Arrange
        var repoMock = new Mock<IAdministrador>();
        var validatorMock = new Mock<IAdminServiceValidator>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();

        var admin = new Administrador("Juan", "Pérez", "jperez", "1234");

        // Simulamos usuario actual
        currentUserMock.Setup(c => c.GetUserId()).Returns(1);

        // Validaciones exitosas
        validatorMock.Setup(v => v.ValidateForUpdate(It.IsAny<UpdateAdminDTO>()))
            .Returns(new ServiceResult(true, "DTO válido para actualizar administrador"));

        validatorMock.Setup(v => v.ValidateAdminExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(true, "Administrador existente", admin));

        // Simulamos que el update es exitoso
        repoMock.Setup(r => r.Update(It.IsAny<Administrador>()))
            .ReturnsAsync(OperationResult.SuccessResult("Administrador actualizado correctamente"));

        var service = new AdminService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        var dto = new UpdateAdminDTO
        {
            AdminId = 1,
            Nombre = "Juan",
            Apellido = "Pérez",
            Username = "jperez"
        };

        // Act
        var result = await service.UpdateAdmin(dto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Administrador actualizado correctamente", result.Message);
        Assert.NotNull(result.Data);
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
        // Arrange
        var repoMock = new Mock<IAdministrador>();
        var validatorMock = new Mock<IAdminServiceValidator>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();

        validatorMock.Setup(v => v.ValidateForDelete(It.IsAny<DeleteAdminDTO>()))
            .Returns(new ServiceResult(true, "DTO válido para eliminar administrador"));

        validatorMock.Setup(v => v.ValidateAdminExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(false, "Administrador no encontrado"));

        var service = new AdminService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        var dto = new DeleteAdminDTO { AdminId = 1 };

        // Act
        var result = await service.RemoveAdmin(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Administrador no encontrado", result.Message);
    }

    [Fact]
    public async Task RemoveAdmin_ShouldFail_WhenRemoveFails()
    {
        // Arrange
        var repoMock = new Mock<IAdministrador>();
        var validatorMock = new Mock<IAdminServiceValidator>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();

        var admin = new Administrador("Juan", "Pérez", "jperez", "1234");

        validatorMock.Setup(v => v.ValidateForDelete(It.IsAny<DeleteAdminDTO>()))
            .Returns(new ServiceResult(true, "DTO válido para eliminar administrador"));

        validatorMock.Setup(v => v.ValidateAdminExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(true, "Administrador existente", admin));

        repoMock.Setup(r => r.Remove(It.IsAny<Administrador>()))
            .ReturnsAsync(OperationResult.FailureResult("Remove error"));

        var service = new AdminService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        var dto = new DeleteAdminDTO { AdminId = 1 };

        // Act
        var result = await service.RemoveAdmin(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Remove error", result.Message);
    }


    [Fact]
    public async Task RemoveAdmin_ShouldSucceed_WhenValid()
    {
        // Arrange
        var repoMock = new Mock<IAdministrador>();
        var validatorMock = new Mock<IAdminServiceValidator>();
        var loggerMock = new Mock<ILogger<AdminService>>();
        var currentUserMock = new Mock<ICurrentUserService>();

        var admin = new Administrador("Pedro", "López", "plopez", "12345");

        validatorMock.Setup(v => v.ValidateForDelete(It.IsAny<DeleteAdminDTO>()))
            .Returns(new ServiceResult(true, "DTO válido para eliminar administrador"));

        validatorMock.Setup(v => v.ValidateAdminExistente(It.IsAny<int>()))
            .ReturnsAsync(new ServiceResult(true, "Administrador existente", admin));

        repoMock.Setup(r => r.Remove(It.IsAny<Administrador>()))
            .ReturnsAsync(OperationResult.SuccessResult("Administrador eliminado correctamente"));

        var service = new AdminService(
            repoMock.Object,
            loggerMock.Object,
            currentUserMock.Object,
            validatorMock.Object
        );

        var dto = new DeleteAdminDTO { AdminId = 1 };

        // Act
        var result = await service.RemoveAdmin(dto);

        // Assert
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