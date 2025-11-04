using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SGCP.Domain.Entities.ModuloDeUsuarios;
using SGCP.Persistence.Repositories.ModuloUsuarios;

namespace SGCP.Test.Persistence.ModuloUsuarios
{
    public class UnitTestClienteRepositoryEF : IDisposable
    {
        private readonly SGCPDbContext _context;
        private readonly Mock<ILogger<ClienteRepositoryEF>> _mockLogger;
        private readonly ClienteRepositoryEF _repository;

        public UnitTestClienteRepositoryEF()
        {
            var options = new DbContextOptionsBuilder<SGCPDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new SGCPDbContext(options);
            _mockLogger = new Mock<ILogger<ClienteRepositoryEF>>();
            _repository = new ClienteRepositoryEF(_context, _mockLogger.Object);
        }

        #region Save Tests

        [Fact]
        public async Task Save_ShouldCreateCliente_Successfully()
        {
            // Arrange
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };

            // Act
            var result = await _repository.Save(cliente);

            // Assert
            Assert.True(result.Success);
            var savedCliente = result.Data as Cliente;
            Assert.NotNull(savedCliente);
            Assert.Equal("cmartinez", savedCliente.Username);
            Assert.True(savedCliente.FechaCreacion != default);
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenRequiredFieldsAreMissing()
        {
            // Arrange
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = null, // Campo requerido
                Username = "cmartinez",
                Password = "password123"
            };

            // Act
            var result = await _repository.Save(cliente);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Error", result.Message);
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenUsernameExceedsMaxLength()
        {
            // Arrange
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = new string('a', 51), 
                Password = "password123"
            };

            // Act
            var result = await _repository.Save(cliente);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Error", result.Message);
        }

        #endregion

        #region GetByUsername Tests

        [Fact]
        public async Task GetByUsername_ShouldReturnCliente_WhenExists()
        {
            // Arrange
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };
            await _context.Cliente.AddAsync(cliente);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByUsername("cmartinez");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("cmartinez", result.Username);
            Assert.Equal("Carlos", result.Nombre);
        }

        [Fact]
        public async Task GetByUsername_ShouldReturnNull_WhenNotExists()
        {
            var result = await _repository.GetByUsername("noexiste");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsername_ShouldReturnNull_WhenUsernameIsNull()
        {
            var result = await _repository.GetByUsername(null);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsername_ShouldReturnNull_WhenUsernameIsEmpty()
        {
            var result = await _repository.GetByUsername("");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsername_ShouldLogInformation()
        {
            await _repository.GetByUsername("cmartinez");

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Buscando cliente")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region GetPedidosByClienteId Tests

        [Fact]
        public async Task GetPedidosByClienteId_ShouldReturnEmptyList_WhenClienteHasNoPedidos()
        {
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };
            await _context.Cliente.AddAsync(cliente);
            await _context.SaveChangesAsync();

            var pedidos = await _repository.GetPedidosByClienteId(cliente.IdUsuario);

            Assert.NotNull(pedidos);
            Assert.Empty(pedidos);
        }

        [Fact]
        public async Task GetPedidosByClienteId_ShouldReturnEmptyList_WhenClienteDoesNotExist()
        {
            var pedidos = await _repository.GetPedidosByClienteId(999);
            Assert.NotNull(pedidos);
            Assert.Empty(pedidos);
        }

        [Fact]
        public async Task GetPedidosByClienteId_ShouldReturnEmptyList_WhenIdIsZero()
        {
            var pedidos = await _repository.GetPedidosByClienteId(0);
            Assert.NotNull(pedidos);
            Assert.Empty(pedidos);
        }

        [Fact]
        public async Task GetPedidosByClienteId_ShouldReturnEmptyList_WhenIdIsNegative()
        {
            var pedidos = await _repository.GetPedidosByClienteId(-1);
            Assert.NotNull(pedidos);
            Assert.Empty(pedidos);
        }

        [Fact]
        public async Task GetPedidosByClienteId_ShouldLogInformation()
        {
            var clienteId = 1;
            await _repository.GetPedidosByClienteId(clienteId);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Obteniendo pedidos")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_ShouldModifyCliente_Successfully()
        {
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };
            await _repository.Save(cliente);

            // Act
            cliente.Password = "newpass";
            var result = await _repository.Update(cliente);

            // Assert
            Assert.True(result.Success);
            var updatedCliente = await _repository.GetByUsername("cmartinez");
            Assert.Equal("newpass", updatedCliente.Password);
            Assert.NotNull(updatedCliente.FechaModificacion);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenClienteDoesNotExist()
        {
            var cliente = new Cliente
            {
                IdUsuario = 999,
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };

            var result = await _repository.Update(cliente);

            Assert.False(result.Success);
            Assert.Contains("Error", result.Message);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenRequiredFieldIsSetToNull()
        {
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };
            await _repository.Save(cliente);

            cliente.Nombre = null;
            var result = await _repository.Update(cliente);

            Assert.False(result.Success);
        }

        #endregion

        #region Remove Tests

        [Fact]
        public async Task Remove_ShouldDeleteCliente_Successfully()
        {
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };
            await _repository.Save(cliente);

            var result = await _repository.Remove(cliente);

            Assert.True(result.Success);
            var deletedCliente = await _repository.GetByUsername("cmartinez");
            Assert.Null(deletedCliente);
        }

        [Fact]
        public async Task Remove_ShouldReturnFailure_WhenClienteDoesNotExist()
        {
            var cliente = new Cliente
            {
                IdUsuario = 999,
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };

            var result = await _repository.Remove(cliente);

            Assert.False(result.Success);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ShouldReturnAllClientes()
        {
            var clientes = new List<Cliente>
            {
                new Cliente { Nombre = "Carlos", Apellido = "Martínez", Username = "cmartinez", Password = "pass1" },
                new Cliente { Nombre = "Ana", Apellido = "Rodríguez", Username = "arodriguez", Password = "pass2" },
                new Cliente { Nombre = "Luis", Apellido = "Fernández", Username = "lfernandez", Password = "pass3" }
            };

            foreach (var cliente in clientes)
            {
                await _repository.Save(cliente);
            }

            var result = await _repository.GetAll();

            Assert.True(result.Success);
            var list = result.Data as List<Cliente>;
            Assert.NotNull(list);
            Assert.Equal(3, list.Count);
        }

        [Fact]
        public async Task GetAll_WithFilter_ShouldReturnFilteredClientes()
        {
            var clientes = new List<Cliente>
            {
                new Cliente { Nombre = "Carlos", Apellido = "Martínez", Username = "cmartinez", Password = "p1" },
                new Cliente { Nombre = "Ana", Apellido = "Martínez", Username = "amartinez", Password = "p2" },
                new Cliente { Nombre = "Luis", Apellido = "Fernández", Username = "lfernandez", Password = "p3" }
            };

            foreach (var cliente in clientes)
            {
                await _repository.Save(cliente);
            }

            var result = await _repository.GetAll(c => c.Apellido == "Martínez");

            Assert.True(result.Success);
            var list = result.Data as List<Cliente>;
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);
            Assert.All(list, c => Assert.Equal("Martínez", c.Apellido));
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoClientes()
        {
            var result = await _repository.GetAll();

            Assert.True(result.Success);
            var list = result.Data as List<Cliente>;
            Assert.NotNull(list);
            Assert.Empty(list);
        }

        #endregion

        #region Exists Tests

        [Fact]
        public async Task Exists_ShouldReturnTrue_WhenClienteExists()
        {
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };
            await _repository.Save(cliente);

            var exists = await _repository.Exists(c => c.Username == "cmartinez");
            Assert.True(exists);
        }

        [Fact]
        public async Task Exists_ShouldReturnFalse_WhenClienteDoesNotExist()
        {
            var exists = await _repository.Exists(c => c.Username == "noexiste");
            Assert.False(exists);
        }

        [Fact]
        public async Task Exists_ShouldWorkWithComplexFilters()
        {
            var cliente = new Cliente
            {
                Nombre = "Carlos",
                Apellido = "Martínez",
                Username = "cmartinez",
                Password = "password123"
            };
            await _repository.Save(cliente);

            var exists = await _repository.Exists(c =>
                c.Username == "cmartinez" && c.Apellido == "Martínez");

            Assert.True(exists);
        }

        #endregion

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }
    }
}