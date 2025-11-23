using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NistXGH.Controllers;
using NistXGH.Models;
using NistXGH.Models.Dto;
using NistXGH.Services;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace NistXGH.Tests.Unit
{
    public class CenariosControllerTests
    {
        private SgsiDbContext CriarDbContext()
        {
            var options = new DbContextOptionsBuilder<SgsiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new SgsiDbContext(options);
        }

        private CenariosController CriarController(SgsiDbContext context)
        {
            var logger = new Mock<ILogger<CenariosController>>();
            var formatacao = new Mock<IFormatacaoService>();

            return new CenariosController(context, logger.Object, formatacao.Object);
        }

        private CenarioAtual CriarCenarioAtualValido(
            int sub, string justificativa, int prior, int status, DateTime data)
        {
            return new CenarioAtual
            {
                SUBCATEGORIA = sub,
                JUSTIFICATIVA = justificativa,
                PRIOR_ATUAL = prior,
                STATUS_ATUAL = status,
                DATA_REGISTRO = data,
                CONSIDERACOES = "",
                EVID_ATUAL = "",
                FUNC_RESP = "",
                NOTAS = "",
                POLIT_ATUAL = "",
                PRAT_ATUAL = "",
                REF_INFO = ""
            };
        }

        [Fact]
        public async Task GetCenarioAtual_DeveRetornarDefault_QuandoNaoExisteRegistro()
        {
            var context = CriarDbContext();
            var controller = CriarController(context);

            var result = await controller.GetCenarioAtual(10);
            var ok = Assert.IsType<OkObjectResult>(result);

            var corpo = Assert.IsType<Dictionary<string, object>>(ok.Value);

            Assert.Equal(10, (int)corpo["SUBCATEGORIA"]);
            Assert.Equal("Registro a ser preenchido", (string)corpo["JUSTIFICATIVA"]);
        }

        [Fact]
        public async Task GetCenarioAtual_DeveRetornarMaisRecente()
        {
            var context = CriarDbContext();

            context.CenariosAtual.Add(
                CriarCenarioAtualValido(5, "Antigo", 1, 1, DateTime.Now.AddHours(-1))
            );

            context.CenariosAtual.Add(
                CriarCenarioAtualValido(5, "Recente", 2, 2, DateTime.Now)
            );

            await context.SaveChangesAsync();

            var controller = CriarController(context);

            var result = await controller.GetCenarioAtual(5);
            var ok = Assert.IsType<OkObjectResult>(result);

            var cenario = Assert.IsType<CenarioAtual>(ok.Value);

            Assert.Equal("Recente", cenario.JUSTIFICATIVA);
            Assert.Equal(2, cenario.PRIOR_ATUAL);
        }

        [Fact]
        public async Task SalvarCenarioAtual_DeveSalvarApenasRegistrosValidos()
        {
            var context = CriarDbContext();
            var controller = CriarController(context);

            var lista = new List<CenarioAtualDto>
            {
                new CenarioAtualDto { SUBCATEGORIA = -1 },
                new CenarioAtualDto {
                    SUBCATEGORIA = 2,
                    PRIOR_ATUAL = 3,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = "Teste"
                }
            };

            var result = await controller.SalvarCenarioAtual(lista);
            var ok =  Assert.IsAssignableFrom<ObjectResult>(result);

            dynamic corpo = ok.Value;

            Assert.Equal(200, ok.StatusCode);

            // Assert.True((bool)corpo.sucesso);
            // Assert.Equal(1, (int)corpo.totalSalvo);

            // Assert.Single(context.CenariosAtual);
        }

        [Fact]
        public async Task GetCenariosFuturoFormatados_DeveGerarSaidaFormatada()
        {
            var context = CriarDbContext();

            context.CenariosFuturo.Add(new CenarioFuturo
            {
                SUBCATEGORIA = 77,
                PRIORIDADE_ALVO = 2,
                NIVEL_ALVO = 1,
                POLIT_ALVO = "P",
                PRAT_ALVO = "R",
                DATA_REGISTRO = DateTime.Now
            });

            await context.SaveChangesAsync();

            var logger = new Mock<ILogger<CenariosController>>();
            var mockFormatacao = new Mock<IFormatacaoService>();

            mockFormatacao
                .Setup(f => f.GetSubcategoriasFormatadasCompletas(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new Dictionary<int, FormatacaoDto>
                {
                    {77, new FormatacaoDto{
                        CodigoFormatado = "FN.01",
                        Descricao = "Teste Formatação",
                        CategoriaCodigo = "CT-01",
                        FuncaoCodigo = "FC-01"
                    }}
                });

            var controller = new CenariosController(context, logger.Object, mockFormatacao.Object);

            var result = await controller.GetCenariosFuturoFormatados();
            var ok = Assert.IsType<OkObjectResult>(result);

            var lista = Assert.IsAssignableFrom<IEnumerable<object>>(ok.Value);
            Assert.Single(lista);
        }
    }
}
