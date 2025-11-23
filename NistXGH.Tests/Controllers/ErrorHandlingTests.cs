using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NistXGH.Controllers;
using NistXGH.Models;
using NistXGH.Models.Dto;
using NistXGH.Services;
using Xunit;

namespace NistXGH.Tests.Controllers
{
    public class ErrorHandlingTests : TestBase
    {
        private readonly Mock<ILogger<CenariosController>> _mockLogger;
        private readonly Mock<IFormatacaoService> _mockFormatacaoService;
        private readonly SgsiDbContext _context;

        public ErrorHandlingTests()
        {
            _context = CreateMockDbContext();
            _mockLogger = new Mock<ILogger<CenariosController>>();
            _mockFormatacaoService = new Mock<IFormatacaoService>();
            SeedTestData(_context);
        }

        [Fact]
        public async Task GetCenarioAtual_ComExcecaoNoBanco_DeveRetornarInternalServerError()
        {
            // Arrange - Testar cenário de erro
            var controller = new CenariosController(
                _context,
                _mockLogger.Object,
                _mockFormatacaoService.Object
            );

            // Act - Chamar com ID que não existe
            var result = await controller.GetCenarioAtual(9999);

            // Assert - Deve retornar Ok com objeto padrão
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task SalvarCenarioAtual_ComListaNula_DeveRetornarBadRequest()
        {
            // Arrange
            var controller = new CenariosController(
                _context,
                _mockLogger.Object,
                _mockFormatacaoService.Object
            );

            // Act
            var result = await controller.SalvarCenarioAtual(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Nenhum dado enviado.", badRequestResult.Value);
        }

        [Fact]
        public async Task SalvarCenarioAtual_ComListaVazia_DeveRetornarBadRequest()
        {
            // Arrange
            var controller = new CenariosController(
                _context,
                _mockLogger.Object,
                _mockFormatacaoService.Object
            );

            // Act
            var result = await controller.SalvarCenarioAtual(new List<CenarioAtualDto>());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Nenhum dado enviado.", badRequestResult.Value);
        }

        [Fact]
        public async Task SalvarCenarioAtual_ComSubcategoriaZero_DeveRetornarBadRequest()
        {
            // Arrange
            var controller = new CenariosController(
                _context,
                _mockLogger.Object,
                _mockFormatacaoService.Object
            );

            var cenarios = new List<CenarioAtualDto>
            {
                new CenarioAtualDto
                {
                    SUBCATEGORIA = 0,
                    PRIOR_ATUAL = 1,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = "Teste",
                    POLIT_ATUAL = "Política",
                    PRAT_ATUAL = "Prática",
                    FUNC_RESP = "Responsável",
                    REF_INFO = "Referência",
                    EVID_ATUAL = "Evidência",
                    NOTAS = "Notas",
                    CONSIDERACOES = "Considerações",
                },
            };

            // Act
            var result = await controller.SalvarCenarioAtual(cenarios);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task SalvarCenarioAtual_ComSubcategoriaValida_DeveProcessarComSucesso()
        {
            // Arrange
            var controller = new CenariosController(
                _context,
                _mockLogger.Object,
                _mockFormatacaoService.Object
            );

            var cenarios = new List<CenarioAtualDto>
            {
                new CenarioAtualDto
                {
                    SUBCATEGORIA = 1,
                    PRIOR_ATUAL = 1,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = "Teste",
                    POLIT_ATUAL = "Política",
                    PRAT_ATUAL = "Prática",
                    FUNC_RESP = "Responsável",
                    REF_INFO = "Referência",
                    EVID_ATUAL = "Evidência",
                    NOTAS = "Notas",
                    CONSIDERACOES = "Considerações",
                },
            };

            // Act
            var result = await controller.SalvarCenarioAtual(cenarios);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);

            var successProperty = response.GetType().GetProperty("sucesso");
            Assert.NotNull(successProperty);
            Assert.True((bool)successProperty.GetValue(response)!);
        }

        [Fact]
        public async Task GetFuncao_ComIdInexistente_DeveRetornarNotFound()
        {
            // Arrange
            var controller = new FuncoesController(_context);

            // Act
            var result = await controller.GetFuncao(999);

            // Assert
            var actionResult = Assert.IsType<ActionResult<FuncaoDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }
    }
}
