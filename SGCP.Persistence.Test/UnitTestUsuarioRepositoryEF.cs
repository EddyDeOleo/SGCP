
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Persistence.Repositories.ModuloUsuarios;

namespace SGCP.Test.Persistence.ModuloUsuarios
{
    public class UnitTestUsuarioRepositoryEF : IDisposable
    {
        private readonly SGCPDbContext _context;
        private readonly Mock<ILogger<UsuarioRepositoryEF>> _mockLogger;
        private readonly UsuarioRepositoryEF _repository;

        public UnitTestUsuarioRepositoryEF()
        {
            var options = new DbContextOptionsBuilder<SGCPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGCPDbContext(options);
            _mockLogger = new Mock<ILogger<UsuarioRepositoryEF>>();
            _repository = new UsuarioRepositoryEF(_context, _mockLogger.Object);
        }

        #region GetByUsername Tests

     

        [Fact]
        public async Task GetByUsername_ShouldReturnNull_WhenUsernameDoesNotExist()
        {
            // Act
            var result = await _repository.GetByUsername("noexiste");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsername_ShouldReturnNull_WhenUsernameIsNull()
        {
            // Act
            var result = await _repository.GetByUsername(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsername_ShouldReturnNull_WhenUsernameIsEmpty()
        {
            // Act
            var result = await _repository.GetByUsername("");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsername_ShouldLogWarning_WhenUsernameNotFound()
        {
            // Act
            await _repository.GetByUsername("noexiste");

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No se encontró usuario")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByUsername_ShouldThrowException_WhenContextIsDisposed()
        {
            var options = new DbContextOptionsBuilder<SGCPDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var disposedContext = new SGCPDbContext(options);
            var repository = new UsuarioRepositoryEF(disposedContext, _mockLogger.Object);

            disposedContext.Dispose();

            // Act & Assert
            await Assert.ThrowsAsync<ObjectDisposedException>(async () =>
                await repository.GetByUsername("jperez"));
        }

      

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoUsuarios()
        {
            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.True(result.Success);
            var list = result.Data as List<Usuario>;
            Assert.NotNull(list);
            Assert.Empty(list);
        }

        #endregion

        #region Exists Tests

       

        [Fact]
        public async Task Exists_ShouldReturnFalse_WhenUsernameDoesNotExist()
        {
            // Act
            var exists = await _repository.Exists(u => u.Username == "noexiste");

            // Assert
            Assert.False(exists);
        }

        #endregion

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }
    }
}