using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Moq;
using SGCP.Domain.Base;
using SGCP.Domain.Entities.ModuloDeProducto;
using SGCP.Persistence.Base;
using SGCP.Persistence.Base.IEntityValidator;
using SGCP.Persistence.Repositories.ModuloProducto;

namespace SGCP.Test.Persistence.ModuloProducto
{
    public class UnitTestProductoRepositoryAdo : IDisposable
    {
        private readonly Mock<IStoredProcedureExecutor> _mockSpExecutor;
        private readonly Mock<ILogger<ProductoRepositoryAdo>> _mockLogger;
        private readonly Mock<IEntityValidator<Producto>> _mockValidator;
        private readonly ProductoRepositoryAdo _repository;

        public UnitTestProductoRepositoryAdo()
        {
            _mockSpExecutor = new Mock<IStoredProcedureExecutor>();
            _mockLogger = new Mock<ILogger<ProductoRepositoryAdo>>();
            _mockValidator = new Mock<IEntityValidator<Producto>>();

            _repository = new ProductoRepositoryAdo(
                _mockSpExecutor.Object,
                _mockLogger.Object,
                _mockValidator.Object
            );
        }

        #region Validación Tests - Save (Insert)

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenNombreIsEmpty()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = "", // ❌ Inválido
                Precio = 100,
                Stock = 10,
                Categoria = "Electrónica"
            };

            // El validador retorna error
            _mockValidator
                .Setup(v => v.ValidateForSave(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("El nombre del producto es obligatorio y no puede exceder 100 caracteres."));

            // Act
            var result = await _repository.Save(producto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("nombre", result.Message.ToLower());

            // Verificar que NO se llamó al SP (la validación detuvo la ejecución)
            _mockSpExecutor.Verify(
                x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<SqlParameter>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenPrecioIsNegative()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = "Laptop",
                Precio = -100, // ❌ Inválido
                Stock = 10,
                Categoria = "Electrónica"
            };

            _mockValidator
                .Setup(v => v.ValidateForSave(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("El precio debe ser mayor a cero."));

            // Act
            var result = await _repository.Save(producto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("precio", result.Message.ToLower());

            _mockSpExecutor.Verify(
                x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<SqlParameter>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenStockIsNegative()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = "Mouse",
                Precio = 50,
                Stock = -5, // ❌ Inválido
                Categoria = "Periféricos"
            };

            _mockValidator
                .Setup(v => v.ValidateForSave(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("El stock no puede ser negativo."));

            // Act
            var result = await _repository.Save(producto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("stock", result.Message.ToLower());

            _mockSpExecutor.Verify(
                x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>(), It.IsAny<SqlParameter>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenNombreExceeds100Characters()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = new string('A', 101), // ❌ 101 caracteres
                Precio = 100,
                Stock = 10,
                Categoria = "Electrónica"
            };

            _mockValidator
                .Setup(v => v.ValidateForSave(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("El nombre del producto es obligatorio y no puede exceder 100 caracteres."));

            // Act
            var result = await _repository.Save(producto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("100 caracteres", result.Message.ToLower());
        }

        [Fact]
        public async Task Save_ShouldReturnFailure_WhenDescripcionExceeds255Characters()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = "Producto Test",
                Descripcion = new string('A', 256), // ❌ 256 caracteres
                Precio = 100,
                Stock = 10,
                Categoria = "Electrónica"
            };

            _mockValidator
                .Setup(v => v.ValidateForSave(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("La descripción no puede exceder 255 caracteres."));

            // Act
            var result = await _repository.Save(producto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("descripción", result.Message.ToLower());
        }

        [Fact]
        public async Task Save_ShouldReturnSuccess_WhenProductoIsValid()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = "Laptop HP",
                Descripcion = "Laptop de alta gama",
                Precio = 1200,
                Stock = 5,
                Categoria = "Electrónica"
            };

            // Validación exitosa
            _mockValidator
                .Setup(v => v.ValidateForSave(It.IsAny<Producto>()))
                .Returns(OperationResult.SuccessResult("Validación exitosa"));

            // Mock del SP Executor
            _mockSpExecutor
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<SqlParameter>()))
                    .ReturnsAsync(1);

            // Act
            var result = await _repository.Save(producto);

            // Assert
            Assert.True(result.Success);

            // Verificar que SÍ se llamó al SP (validación pasó)
            _mockSpExecutor.Verify(
                x => x.ExecuteAsync(
                    It.Is<string>(sp => sp == "sp_InsertProducto"),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<SqlParameter>()),
                Times.Once
            );
        }

        #endregion

        #region Validación Tests - Update

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenIdProductoIsZero()
        {
            // Arrange
            var producto = new Producto
            {
                IdProducto = 0, // ❌ Inválido
                Nombre = "Laptop",
                Precio = 1200,
                Stock = 10,
                Categoria = "Electrónica"
            };

            _mockValidator
                .Setup(v => v.ValidateForUpdate(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("El Id del producto debe ser válido para actualizar."));

            // Act
            var result = await _repository.Update(producto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Id", result.Message);

            _mockSpExecutor
      .Setup(x => x.ExecuteAsync(
          It.IsAny<string>(),
          It.IsAny<Dictionary<string, object>>(),
          It.IsAny<SqlParameter?>()
      ))
      .ReturnsAsync(1);
        }

        [Fact]
        public async Task Update_ShouldReturnFailure_WhenNombreIsEmpty()
        {
            // Arrange
            var producto = new Producto
            {
                IdProducto = 1,
                Nombre = "", // ❌ Inválido
                Precio = 1200,
                Stock = 10,
                Categoria = "Electrónica"
            };

            _mockValidator
                .Setup(v => v.ValidateForUpdate(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("El nombre del producto es obligatorio y no puede exceder 100 caracteres."));

            // Act
            var result = await _repository.Update(producto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("nombre", result.Message.ToLower());
        }

        [Fact]
        public async Task Update_ShouldReturnSuccess_WhenProductoIsValid()
        {
            // Arrange
            var producto = new Producto
            {
                IdProducto = 1,
                Nombre = "Laptop Dell",
                Precio = 1500,
                Stock = 8,
                Categoria = "Electrónica"
            };

            _mockValidator
                .Setup(v => v.ValidateForUpdate(It.IsAny<Producto>()))
                .Returns(OperationResult.SuccessResult("Validación exitosa"));

            _mockSpExecutor
          .Setup(x => x.ExecuteAsync(
              It.IsAny<string>(),
              It.IsAny<Dictionary<string, object>>(),
              It.IsAny<SqlParameter?>()
          ))
          .ReturnsAsync(1);

            // Act
            var result = await _repository.Update(producto);

            // Assert
            Assert.True(result.Success);

            _mockSpExecutor
     .Setup(x => x.ExecuteAsync(
         It.IsAny<string>(),
         It.IsAny<Dictionary<string, object>>(),
         It.IsAny<SqlParameter?>()
     ))
     .ReturnsAsync(1);
        }

        #endregion

        #region Validación Tests - Remove (Delete)

        [Fact]
        public async Task Remove_ShouldReturnFailure_WhenProductoIsNull()
        {
            // Arrange
            Producto producto = null;

            _mockValidator
                .Setup(v => v.ValidateForRemove(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("El producto no puede ser nulo."));

            // Act
            var result = await _repository.Remove(producto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("nulo", result.Message.ToLower());

            _mockSpExecutor
      .Setup(x => x.ExecuteAsync(
          It.IsAny<string>(),
          It.IsAny<Dictionary<string, object>>(),
          It.IsAny<SqlParameter?>()
      ))
      .ReturnsAsync(1);
        }

        [Fact]
        public async Task Remove_ShouldReturnFailure_WhenIdProductoIsInvalid()
        {
            // Arrange
            var producto = new Producto
            {
                IdProducto = -1 // ❌ Inválido
            };

            _mockValidator
                .Setup(v => v.ValidateForRemove(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("El Id del producto debe ser válido para eliminar."));

            // Act
            var result = await _repository.Remove(producto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Id", result.Message);
        }

        [Fact]
        public async Task Remove_ShouldReturnSuccess_WhenProductoIsValid()
        {
            // Arrange
            var producto = new Producto
            {
                IdProducto = 1,
                Nombre = "Laptop"
            };

            _mockValidator
                .Setup(v => v.ValidateForRemove(It.IsAny<Producto>()))
                .Returns(OperationResult.SuccessResult("Validación exitosa"));

            _mockSpExecutor
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<SqlParameter?>()
                ))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.Remove(producto);

            // Assert
            Assert.True(result.Success);

            _mockSpExecutor
      .Setup(x => x.ExecuteAsync(
          It.IsAny<string>(),
          It.IsAny<Dictionary<string, object>>(),
          It.IsAny<SqlParameter?>()
      ))
      .ReturnsAsync(1);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoProductos()
        {
            // Arrange
            _mockSpExecutor
                .Setup(x => x.QueryAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, Producto>>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new List<Producto>());

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.True(result.Success);
            var list = result.Data as List<Producto>;
            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetAll_ShouldReturnProductos_WhenProductosExist()
        {
            // Arrange
            var productos = new List<Producto>
            {
                new Producto { IdProducto = 1, Nombre = "Laptop", Precio = 1200 },
                new Producto { IdProducto = 2, Nombre = "Mouse", Precio = 50 }
            };

            _mockSpExecutor
                .Setup(x => x.QueryAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, Producto>>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(productos);

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.True(result.Success);
            var list = result.Data as List<Producto>;
            Assert.NotNull(list);
            Assert.Equal(2, list.Count);
        }

        #endregion

        #region GetEntityBy Tests

        [Fact]
        public async Task GetEntityBy_ShouldReturnFailure_WhenProductoNotFound()
        {
            // Arrange
            _mockSpExecutor
                .Setup(x => x.QueryAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, Producto>>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new List<Producto>());

            // Act
            var result = await _repository.GetEntityBy(999);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("no encontrado", result.Message.ToLower());
        }

        [Fact]
        public async Task GetEntityBy_ShouldReturnSuccess_WhenProductoExists()
        {
            // Arrange
            var producto = new Producto
            {
                IdProducto = 1,
                Nombre = "Laptop HP",
                Precio = 1200
            };

            _mockSpExecutor
                .Setup(x => x.QueryAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<SqlDataReader, Producto>>(),
                    It.IsAny<Dictionary<string, object>>()))
                .ReturnsAsync(new List<Producto> { producto });

            // Act
            var result = await _repository.GetEntityBy(1);

            // Assert
            Assert.True(result.Success);
            var productoResult = result.Data as Producto;
            Assert.NotNull(productoResult);
            Assert.Equal(1, productoResult.IdProducto);
            Assert.Equal("Laptop HP", productoResult.Nombre);
        }

        #endregion

        #region Logging Tests

        [Fact]
        public async Task Save_ShouldLogWarning_WhenValidationFails()
        {
            // Arrange
            var producto = new Producto { Nombre = "", Precio = -10 };

            _mockValidator
                .Setup(v => v.ValidateForSave(It.IsAny<Producto>()))
                .Returns(OperationResult.FailureResult("Validación falló"));

            // Act
            await _repository.Save(producto);

            // Assert 
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Validación")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);
        }

        #endregion

        #region Validator Called Tests

        [Fact]
        public async Task Save_ShouldCallValidator_Always()
        {
            // Arrange
            var producto = new Producto
            {
                Nombre = "Test",
                Precio = 100,
                Stock = 10,
                Categoria = "Test"
            };

            _mockValidator
                .Setup(v => v.ValidateForSave(It.IsAny<Producto>()))
                .Returns(OperationResult.SuccessResult("OK"));

            _mockSpExecutor
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<SqlParameter>()))
                    .ReturnsAsync(1);

            // Act
            await _repository.Save(producto);

            // Assert - Verificar que el validador fue llamado
            _mockValidator.Verify(
                v => v.ValidateForSave(It.IsAny<Producto>()),
                Times.Once,
                "El validador DEBE ser llamado antes de guardar"
            );
        }

        [Fact]
        public async Task Update_ShouldCallValidator_Always()
        {
            // Arrange
            var producto = new Producto
            {
                IdProducto = 1,
                Nombre = "Test",
                Precio = 100
            };

            _mockValidator
                .Setup(v => v.ValidateForUpdate(It.IsAny<Producto>()))
                .Returns(OperationResult.SuccessResult("OK"));
            _mockSpExecutor
                .Setup(x => x.ExecuteAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>(),
                    It.IsAny<SqlParameter?>()
                ))
                .ReturnsAsync(1);

            // Act
            await _repository.Update(producto);

            // Assert
            _mockValidator.Verify(
                v => v.ValidateForUpdate(It.IsAny<Producto>()),
                Times.Once,
                "El validador DEBE ser llamado antes de actualizar"
            );
        }

        [Fact]
        public async Task Remove_ShouldCallValidator_Always()
        {
            // Arrange
            var producto = new Producto { IdProducto = 1 };

            _mockValidator
                .Setup(v => v.ValidateForRemove(It.IsAny<Producto>()))
                .Returns(OperationResult.SuccessResult("OK"));

            _mockSpExecutor
      .Setup(x => x.ExecuteAsync(
          It.IsAny<string>(),
          It.IsAny<Dictionary<string, object>>(),
          It.IsAny<SqlParameter?>()
      ))
      .ReturnsAsync(1);

            // Act
            await _repository.Remove(producto);

            // Assert
            _mockValidator.Verify(
                v => v.ValidateForRemove(It.IsAny<Producto>()),
                Times.Once,
                "El validador DEBE ser llamado antes de eliminar"
            );
        }

        #endregion

        public void Dispose()
        {
        }
    }
}