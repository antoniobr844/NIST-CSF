// NistXGH.Tests/Services/FormatacaoServiceTests.cs
using Microsoft.Extensions.Logging;
using Moq;
using NistXGH.Services;
using Xunit;

namespace NistXGH.Tests.Services
{
    public class FormatacaoServiceTests : TestBase
    {
        private readonly FormatacaoService _service;
        private readonly Mock<ILogger<FormatacaoService>> _mockLogger;
        private readonly SgsiDbContext _context;

        public FormatacaoServiceTests()
        {
            _context = CreateMockDbContext();
            _mockLogger = new Mock<ILogger<FormatacaoService>>();

            // 🔥 CORREÇÃO: Ordem correta dos parâmetros
            _service = new FormatacaoService(_context, _mockLogger.Object);

            SeedTestData(_context);
        }

        [Fact]
        public async Task FormatSubcategoria_WithValidId_ReturnsFormattedString()
        {
            // Arrange
            var subcategoriaId = 1;

            // Act
            var result = await _service.FormatSubcategoria(subcategoriaId);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("GV.AC", result); // Verifica se contém o código formatado
        }

        [Fact]
        public async Task FormatSubcategoria_WithInvalidId_ReturnsErrorMessage()
        {
            // Arrange
            var subcategoriaId = 999;

            // Act
            var result = await _service.FormatSubcategoria(subcategoriaId);

            // Assert
            Assert.NotNull(result);

            Assert.Contains("N/A", result);
        }
    }
}
