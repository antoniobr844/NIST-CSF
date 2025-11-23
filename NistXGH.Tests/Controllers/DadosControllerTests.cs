using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NistXGH.Controllers;
using NistXGH.Models;
using Xunit;

namespace NistXGH.Tests.Controllers
{
    public class DadosControllerTests : TestBase
    {
        private readonly Mock<ILogger<DadosController>> _mockLogger;
        private readonly DadosController _controller;
        private readonly SgsiDbContext _context;

        public DadosControllerTests()
        {
            _context = CreateMockDbContext();
            _mockLogger = new Mock<ILogger<DadosController>>();
            _controller = new DadosController(_context, _mockLogger.Object);

            SeedTestData(_context); // Usa os dados do TestBase
        }

        [Fact]
        public async Task GetPrioridades_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetPrioridades();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<PrioridadeTb>>(okResult.Value);
            Assert.Equal(3, returnValue.Count);
        }

        [Fact]
        public async Task GetStatus_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetStatus();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<StatusTb>>(okResult.Value);
            Assert.Equal(3, returnValue.Count);
        }

        [Fact]
        public async Task GetPrioridade_WithValidId_ReturnsPrioridade()
        {
            // Act
            var result = await _controller.GetPrioridade(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var prioridade = Assert.IsType<PrioridadeTb>(okResult.Value);
            Assert.Equal("1", prioridade.NIVEL);
        }

        [Fact]
        public async Task GetPrioridade_WithInvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetPrioridade(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // DadosControllerTests.cs - CORREÇÃO DO TESTE FALHANDO
        [Fact]
        public async Task GetTodosCatalogos_ReturnsBothPrioridadesAndStatus()
        {
            // Act
            var result = await _controller.GetTodosCatalogos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            // Em vez de dynamic, vamos usar reflexão para verificar as propriedades
            var returnValue = okResult.Value;
            Assert.NotNull(returnValue);

            // Verifica se o objeto tem as propriedades esperadas
            var type = returnValue.GetType();
            var prioridadesProp = type.GetProperty("prioridades");
            var statusProp = type.GetProperty("status");

            Assert.NotNull(prioridadesProp);
            Assert.NotNull(statusProp);

            var prioridades = prioridadesProp.GetValue(returnValue) as List<PrioridadeTb>;
            var status = statusProp.GetValue(returnValue) as List<StatusTb>;

            Assert.NotNull(prioridades);
            Assert.NotNull(status);
            Assert.Equal(3, prioridades.Count);
            Assert.Equal(3, status.Count);
        }
    }
}
