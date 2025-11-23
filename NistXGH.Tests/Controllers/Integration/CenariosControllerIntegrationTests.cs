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
    public class CenariosControllerIntegrationTests : TestBase // 🔥 Adicionar herança
    {
        private readonly Mock<ILogger<CenariosController>> _mockLogger;
        private readonly Mock<IFormatacaoService> _mockFormatacaoService;
        private readonly SgsiDbContext _context; // 🔥 Adicionar campo
        private readonly CenariosController _controller;

        public CenariosControllerIntegrationTests()
        {
            _context = CreateMockDbContext(); // 🔥 Criar contexto
            _mockLogger = new Mock<ILogger<CenariosController>>();
            _mockFormatacaoService = new Mock<IFormatacaoService>();
            _controller = new CenariosController(
                _context,
                _mockLogger.Object,
                _mockFormatacaoService.Object
            );

            SeedTestData(_context); // 🔥 Popular dados
        }

        [Fact]
        public async Task SalvarCenarioAtual_ComDadosValidos_DeveCriarNovoRegistro()
        {
            // Arrange
            var cenarios = new List<CenarioAtualDto>
            {
                new CenarioAtualDto
                {
                    SUBCATEGORIA = 1,
                    PRIOR_ATUAL = 2,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = "Teste de integração",
                    POLIT_ATUAL = "Política teste",
                    PRAT_ATUAL = "Prática teste",
                    FUNC_RESP = "Responsável",
                    REF_INFO = "Referência",
                    EVID_ATUAL = "Evidência",
                    NOTAS = "Notas",
                    CONSIDERACOES = "Considerações",
                },
            };

            // Act
            var result = await _controller.SalvarCenarioAtual(cenarios);

            // Assert - Usar tipo explícito
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);

            var successProperty = response.GetType().GetProperty("sucesso");
            Assert.NotNull(successProperty);
            Assert.True((bool)successProperty.GetValue(response));

            var totalSalvoProperty = response.GetType().GetProperty("totalSalvo");
            Assert.NotNull(totalSalvoProperty);
            Assert.Equal(1, (int)totalSalvoProperty.GetValue(response));

            var cenarioSalvo = await _context
                .CenariosAtual.Where(c => c.SUBCATEGORIA == 1)
                .FirstOrDefaultAsync();
            Assert.NotNull(cenarioSalvo);
            Assert.Equal(2, cenarioSalvo.PRIOR_ATUAL);
            Assert.Equal("Teste de integração", cenarioSalvo.JUSTIFICATIVA);
        }

        [Fact]
        public async Task GetCenariosAtualFormatados_DeveRetornarDadosFormatados()
        {
            // Arrange - Criar dados COMPLETOS de cenário atual
            _context.CenariosAtual.Add(
                new CenarioAtual
                {
                    SUBCATEGORIA = 1,
                    PRIOR_ATUAL = 1,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = "Justificativa teste",
                    POLIT_ATUAL = "Política teste",
                    PRAT_ATUAL = "Prática teste",
                    FUNC_RESP = "Responsável teste",
                    REF_INFO = "Referência teste",
                    EVID_ATUAL = "Evidência teste",
                    NOTAS = "Notas teste",
                    CONSIDERACOES = "Considerações teste",
                    DATA_REGISTRO = DateTime.Now,
                }
            );
            await _context.SaveChangesAsync();

            // Setup do serviço de formatação
            _mockFormatacaoService
                .Setup(x => x.GetSubcategoriasFormatadasCompletas(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(
                    new Dictionary<int, FormatacaoDto>
                    {
                        [1] = new FormatacaoDto
                        {
                            CodigoFormatado = "GV.AC-1",
                            Descricao = "Descrição formatada",
                            FuncaoCodigo = "GV",
                            CategoriaCodigo = "AC",
                        },
                    }
                );

            // Act
            var result = await _controller.GetCenariosAtualFormatados();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var cenariosFormatados = okResult.Value as IEnumerable<object>;
            Assert.NotNull(cenariosFormatados);

            var lista = cenariosFormatados.Cast<object>().ToList();
            Assert.Single(lista);
        }

        [Fact]
        public async Task SalvarCenarioFuturo_ComDadosValidos_DeveCriarRegistro()
        {
            // Arrange
            var cenarios = new List<CenarioFuturoDto>
            {
                new CenarioFuturoDto
                {
                    SUBCATEGORIA = 1,
                    PRIORIDADE_ALVO = 3,
                    NIVEL_ALVO = 2,
                    POLIT_ALVO = "Política futura",
                },
            };

            // Act
            var result = await _controller.SalvarCenarioFuturo(cenarios);

            // Assert - Usar tipo explícito em vez de dynamic
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value;
            Assert.NotNull(response);

            // Usar reflexão para verificar propriedades
            var successProperty = response.GetType().GetProperty("sucesso");
            Assert.NotNull(successProperty);
            Assert.True((bool)successProperty.GetValue(response));

            var cenarioFuturo = await _context
                .CenariosFuturo.Where(c => c.SUBCATEGORIA == 1)
                .FirstOrDefaultAsync();
            Assert.NotNull(cenarioFuturo);
            Assert.Equal(3, cenarioFuturo.PRIORIDADE_ALVO);
        }
    }
}
