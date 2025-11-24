// NistXGH.Tests/Controllers/CenariosControllerTests.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NistXGH.Controllers;
using NistXGH.Models;
using NistXGH.Models.Dto;
using NistXGH.Services;
using Xunit;

namespace NistXGH.Tests.Controllers
{
    public class CenariosControllerTests : TestBase
    {
        private readonly Mock<ILogger<CenariosController>> _mockLogger;
        private readonly Mock<IFormatacaoService> _mockFormatacaoService;
        private readonly CenariosController _controller;
        private readonly SgsiDbContext _context;

        public CenariosControllerTests()
        {
            _context = CreateMockDbContext();
            _mockLogger = new Mock<ILogger<CenariosController>>();
            _mockFormatacaoService = new Mock<IFormatacaoService>();

            _controller = new CenariosController(
                _context,
                _mockLogger.Object,
                _mockFormatacaoService.Object
            );
        }

        [Fact]
        public async Task GetCenarioAtual_WithValidSubcategoriaId_ReturnsOkResult()
        {
            // Arrange
            var subcategoriaId = 1;

            // Act
            var result = await _controller.GetCenarioAtual(subcategoriaId);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetCenarioAtual_WithInvalidSubcategoriaId_ReturnsDefaultObject()
        {
            // Arrange
            var subcategoriaId = 999;

            // Act
            var result = await _controller.GetCenarioAtual(subcategoriaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
