using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            SeedTestData(_context);
        }

        [Fact]
        public async Task GetSubcategorias_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetSubcategorias(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var subcategorias = Assert.IsType<List<Subcategorias>>(okResult.Value);
            Assert.Equal(2, subcategorias.Count);
        }

        [Fact]
        public async Task GetSubcategoria_ReturnsOkResult_WhenIdExists()
        {
            // Act
            var result = await _controller.GetSubcategoria(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetSubcategoria_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Act
            var result = await _controller.GetSubcategoria(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetSubcategorias_WithCategoriaId_ReturnsFilteredResult()
        {
            // Act
            var result = await _controller.GetSubcategorias(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var subcategorias = Assert.IsType<List<Subcategorias>>(okResult.Value);
            Assert.Equal(2, subcategorias.Count);
            Assert.All(subcategorias, s => Assert.Equal(1, s.CATEGORIA));
        }
    }
}
