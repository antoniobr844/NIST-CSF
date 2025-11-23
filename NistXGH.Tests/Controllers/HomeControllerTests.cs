using Microsoft.AspNetCore.Mvc;
using NistXGH.Controllers;
using Xunit;

namespace NistXGH.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _controller = new HomeController();
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Consultas_ReturnsViewResult()
        {
            // Act
            var result = _controller.Consultas();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Relatorios_SetsViewDataTitle()
        {
            // Act
            var result = _controller.Relatorios() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Relatórios - Comparação de Cenários", result.ViewData["Title"]);
        }
    }
}
