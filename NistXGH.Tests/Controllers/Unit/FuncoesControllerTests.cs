using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NistXGH.Controllers;
using NistXGH.Models.Dto;
using Xunit;

namespace NistXGH.Tests.Controllers
{
    public class FuncoesControllerTests : TestBase
    {
        private readonly FuncoesController _controller;
        private readonly SgsiDbContext _context;

        public FuncoesControllerTests()
        {
            _context = CreateMockDbContext();
            _controller = new FuncoesController(_context);
        }

        [Fact]
        public async Task GetFuncoes_ReturnsOkResult()
        {
            // Act
            var result = await _controller.GetFuncoes();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var funcoes = Assert.IsType<List<FuncaoDto>>(okResult.Value);
            Assert.Equal(2, funcoes.Count);
        }

        [Fact]
        public async Task GetFuncao_ReturnsValidResult_WhenIdExists()
        {
            // Act
            var result = await _controller.GetFuncao(1);

            // Assert
            var actionResult = Assert.IsType<ActionResult<FuncaoDto>>(result);

            // Para ActionResult<T>, o Result contém o OkObjectResult
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var funcao = Assert.IsType<FuncaoDto>(okResult.Value);

            Assert.NotNull(funcao);
            Assert.Equal(1, funcao.Id);
            Assert.Equal("GV", funcao.Codigo);
        }

        [Fact]
        public async Task GetFuncao_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Act
            var result = await _controller.GetFuncao(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<FuncaoDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}
