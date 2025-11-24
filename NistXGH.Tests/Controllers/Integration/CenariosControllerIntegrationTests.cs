using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NistXGH.Controllers;
using NistXGH.Models;
using NistXGH.Models.Dto;
using NistXGH.Services;
using Xunit;

namespace NistXGH.Tests.Controllers.Integration
{
    public class CenariosControllerIntegrationTests : TestBase
    {
        private readonly Mock<ILogger<CenariosController>> _mockLogger;
        private readonly Mock<IFormatacaoService> _mockFormatacaoService;
        private readonly SgsiDbContext _context;
        private readonly CenariosController _controller;

        public CenariosControllerIntegrationTests()
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
        public async Task SalvarCenarioAtual_ComDadosValidos_DeveCriarNovoRegistro()
        {
            var uniqueSubcategoriaId = GenerateTestId();
            var uniqueJustificativa = GenerateTestString("Justificativa");

            var cenarios = new List<CenarioAtualDto>
            {
                new CenarioAtualDto
                {
                    SUBCATEGORIA = uniqueSubcategoriaId,
                    PRIOR_ATUAL = 2,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = uniqueJustificativa,
                    POLIT_ATUAL = GenerateTestString("Politica"),
                    PRAT_ATUAL = GenerateTestString("Pratica"),
                    FUNC_RESP = GenerateTestString("Responsavel"),
                    REF_INFO = GenerateTestString("Referencia"),
                    EVID_ATUAL = GenerateTestString("Evidencia"),
                    NOTAS = GenerateTestString("Notas"),
                    CONSIDERACOES = GenerateTestString("Consideracoes"),
                },
            };

            var result = await _controller.SalvarCenarioAtual(cenarios);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);

            var successProperty = response.GetType().GetProperty("sucesso");
            Assert.NotNull(successProperty);
            Assert.True((bool)successProperty.GetValue(response)!); // 🔥 ADICIONAR !

            var cenarioSalvo = await _context.CenariosAtual.FirstOrDefaultAsync(c =>
                c.SUBCATEGORIA == uniqueSubcategoriaId
            );
            Assert.NotNull(cenarioSalvo);
            Assert.Equal(2, cenarioSalvo.PRIOR_ATUAL);
            Assert.Equal(uniqueJustificativa, cenarioSalvo.JUSTIFICATIVA);
        }

        // 🔥 CORRIGIR OS OUTROS TESTES COM O MESMO PROBLEMA
        [Fact]
        public async Task SalvarCenarioFuturo_ComDadosValidos_DeveCriarRegistro()
        {
            var uniqueSubcategoriaId = GenerateTestId();

            var cenarios = new List<CenarioFuturoDto>
            {
                new CenarioFuturoDto
                {
                    SUBCATEGORIA = uniqueSubcategoriaId,
                    PRIORIDADE_ALVO = 3,
                    NIVEL_ALVO = 2,
                    POLIT_ALVO = "Política futura",
                },
            };

            var result = await _controller.SalvarCenarioFuturo(cenarios);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);

            var successProperty = response.GetType().GetProperty("sucesso");
            Assert.NotNull(successProperty);
            Assert.True((bool)successProperty.GetValue(response)!); // 🔥 ADICIONAR !

            var cenarioFuturo = await _context.CenariosFuturo.FirstOrDefaultAsync(c =>
                c.SUBCATEGORIA == uniqueSubcategoriaId
            );
            Assert.NotNull(cenarioFuturo);
            Assert.Equal(3, cenarioFuturo.PRIORIDADE_ALVO);
        }
    }
}
