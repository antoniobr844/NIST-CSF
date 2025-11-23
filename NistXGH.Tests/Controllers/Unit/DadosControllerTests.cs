// NistXGH.Tests/Controllers/DadosControllerTests.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NistXGH.Controllers;
using NistXGH.Models;
using Xunit;

namespace NistXGH.Tests.Controllers
{
    public class DadosControllerTests : TestBase
    {
        private readonly DadosController _controller;
        private readonly SgsiDbContext _context;
        private readonly Mock<ILogger<DadosController>> _mockLogger;

        public DadosControllerTests()
        {
            _context = CreateMockDbContext();
            _mockLogger = new Mock<ILogger<DadosController>>();
            _controller = new DadosController(_context, _mockLogger.Object);
            SeedTestData(_context);
        }

        [Fact]
        public async Task GetPrioridades_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetPrioridades();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var prioridades = Assert.IsType<List<PrioridadeTb>>(okResult.Value);
            Assert.Equal(3, prioridades.Count); 
        }

        [Fact]
        public async Task GetPrioridade_WithValidId_ReturnsPrioridade()
        {
            // Act
            var result = await _controller.GetPrioridade(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var prioridade = Assert.IsType<PrioridadeTb>(okResult.Value);
            Assert.Equal("ALTA", prioridade.NIVEL);
        }

        [Fact]
        public async Task GetPrioridade_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetPrioridade(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result); // 🔥 CORREÇÃO: NotFoundObjectResult
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task GetStatus_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var status = Assert.IsType<List<StatusTb>>(okResult.Value);
            Assert.Equal(3, status.Count); // 🔥 CORREÇÃO: 3 status no seed
        }

        [Fact]
        public async Task GetTodosCatalogos_ReturnsBothPrioridadesAndStatus()
        {
            // Act
            var result = await _controller.GetTodosCatalogos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            
            var catalogos = okResult.Value;
            Assert.NotNull(catalogos);

            var prioridadesProperty = catalogos.GetType().GetProperty("prioridades");
            var statusProperty = catalogos.GetType().GetProperty("status");

            Assert.NotNull(prioridadesProperty);
            Assert.NotNull(statusProperty);

            var prioridades = prioridadesProperty.GetValue(catalogos) as List<PrioridadeTb>;
            var status = statusProperty.GetValue(catalogos) as List<StatusTb>;

            Assert.NotNull(prioridades);
            Assert.NotNull(status);
            Assert.Equal(3, prioridades.Count); 
            Assert.Equal(3, status.Count); 
        }
    }
}
