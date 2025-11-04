using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Persistence.Repositories.ModuloUsuarios;

namespace SGCP.Test.Persistence.ModuloUsuarios
{
    public class UnitTestAdministradorRepositoryEF : IDisposable
    {
        private readonly SGCPDbContext _context;
        private readonly Mock<ILogger<AdministradorRepositoryEF>> _mockLogger;
        private readonly AdministradorRepositoryEF _repository;

        public UnitTestAdministradorRepositoryEF()
        {
            var options = new DbContextOptionsBuilder<SGCPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGCPDbContext(options);
            _mockLogger = new Mock<ILogger<AdministradorRepositoryEF>>();
            _repository = new AdministradorRepositoryEF(_context, _mockLogger.Object);
        }

        #region Save Tests

        [Fact]
        public async Task Save_ShouldCreateAdministrador_Successfully()
        {
            // Arrange
            var admin = new Administrador
            {
                Nombre = "Roberto",
                Apellido = "González",
                Username = "rgonzalez",
                Password = "admin123"
            };

            // Act
            var result = await _repository.Save(admin);

            // Assert
            Assert.True(result.Success);
            var savedAdmin = result.Data as Administrador;
            Assert.NotNull(savedAdmin);
            Assert.Equal("rgonzalez", savedAdmin.Username);
            Assert.Equal("Roberto", savedAdmin.Nombre);
            Assert.True(savedAdmin.FechaCreacion != default);
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            var admin = new Administrador
            {
                Nombre = null, // Campo requerido
                Apellido = "González",
                Username = "rgonzalez",
                Password = "admin123"
            };

            // Act
            var result = await _repository.Save(admin);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenUsernameExceedsMaxLength()
        {
            // Arrange
            var admin = new Administrador
            {
                Nombre = "Roberto",
                Apellido = "González",
                Username = new string('a', 51), 
                Password = "admin123"
            };

            // Act
            var result = await _repository.Save(admin);

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenPasswordExceedsMaxLength()
        {
            // Arrange
            var admin = new Administrador
            {
                Nombre = "Roberto",
                Apellido = "González",
                Username = "rgonzalez",
                Password = new string('a', 256) 
            };

            // Act
            var result = await _repository.Save(admin);

            // Assert
            Assert.False(result.Success);
        }

        #endregion

        #region GetByUsername Tests

        [Fact]
        public async Task GetByUsername_ShouldReturnAdministrador_WhenExists()
        {
            // Arrange
            var admin = new Administrador
            {
                Nombre = "Roberto",
                Apellido = "González",
                Username = "rgonzalez",
                Password = "admin123"
            };
            await _context.Administrador.AddAsync(admin);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUsername("rgonzalez");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("rgonzalez", result.Username);
            Assert.Equal("Roberto", result.Nombre);
        }

        [Fact]
        public async Task GetByUsername_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetByUsername("noexiste");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsername_ShouldReturnNull_WhenUsernameIsEmpty()
        {
            var result = await _repository.GetByUsername("");
            Assert.Null(result);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ShouldModifyAdministrador_Successfully()
        {
            // Arrange
            var admin = new Administrador
            {
                Nombre = "Roberto",
                Apellido = "González",
                Username = "rgonzalez",
                Password = "admin123"
            };
            await _repository.Save(admin);

            // Act
            admin.Nombre = "Carlos";
            var result = await _repository.Update(admin);

            // Assert
            Assert.True(result.Success);
            var updatedAdmin = await _repository.GetByUsername("rgonzalez");
            Assert.Equal("Carlos", updatedAdmin.Nombre);
            Assert.NotNull(updatedAdmin.FechaModificacion);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenAdminDoesNotExist()
        {
            var admin = new Administrador
            {
                IdUsuario = 999,
                Nombre = "Roberto",
                Apellido = "González",
                Username = "rgonzalez",
                Password = "admin123"
            };

            var result = await _repository.Update(admin);

            Assert.False(result.Success);
        }

        #endregion

        #region Remove Tests

        [Fact]
        public async Task Remove_ShouldDeleteAdministrador_Successfully()
        {
            // Arrange
            var admin = new Administrador
            {
                Nombre = "Roberto",
                Apellido = "González",
                Username = "rgonzalez",
                Password = "admin123"
            };
            await _repository.Save(admin);

            // Act
            var result = await _repository.Remove(admin);

            // Assert
            Assert.True(result.Success);
            var deletedAdmin = await _repository.GetByUsername("rgonzalez");
            Assert.Null(deletedAdmin);
        }

        #endregion

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }
    }
}