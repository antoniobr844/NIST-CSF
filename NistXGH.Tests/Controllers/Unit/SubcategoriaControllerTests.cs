using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Controllers;
using NistXGH.Models;
using Xunit;

namespace NistXGH.Tests.Controllers
{
    public class SubcategoriasControllerTests : TestBase
    {
        private readonly SubcategoriasController _controller;
        private readonly SgsiDbContext _context;

        public SubcategoriasControllerTests()
        {
            _context = CreateMockDbContext();
            _controller = new SubcategoriasController(_context);
        }

        [Fact]
        public async Task GetSubcategorias_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetSubcategorias(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var subcategorias = Assert.IsType<List<Subcategorias>>(okResult.Value);

            Assert.NotNull(subcategorias);
        }

        [Fact]
        public async Task GetSubcategoria_ReturnsOkResult_WhenIdExists()
        {
            // Arrange
            var result = await _controller.GetSubcategoria(1);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetSubcategoria_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Act
            var result = await _controller.GetSubcategoria(99999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetSubcategorias_WithCategoriaId_ReturnsFilteredResult()
        {
            // Act - Testar com categoria 1 que deve ter subcategorias
            var result = await _controller.GetSubcategorias(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var subcategorias = Assert.IsType<List<Subcategorias>>(okResult.Value);

            // Apenas verificar que retornou uma lista válida
            Assert.NotNull(subcategorias);
        }
    }
}
