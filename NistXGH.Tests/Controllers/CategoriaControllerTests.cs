using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NistXGH.Controllers;
using NistXGH.Models;
using Xunit;

namespace NistXGH.Tests.Controllers
{
    public class CategoriasControllerTests : TestBase
    {
        private readonly CategoriasController _controller;
        private readonly SgsiDbContext _context;

        public CategoriasControllerTests()
        {
            _context = CreateMockDbContext();
            _controller = new CategoriasController(_context);
            SeedTestData(_context);
        }

        [Fact]
        public async Task GetCategorias_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetCategorias(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Categorias>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetCategorias_WithFuncaoId_ReturnsFilteredResult()
        {
            // Act
            var result = await _controller.GetCategorias(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Categorias>>(okResult.Value);
            Assert.All(returnValue, cat => Assert.Equal(1, cat.FUNCAO));
        }
    }
}
