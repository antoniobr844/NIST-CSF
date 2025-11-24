// Testes de API para Cenários (Atual vs Futuro)
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NistXGH.Models.Dto;
using Xunit;

namespace NistXGH.Tests.Integration
{
    public class CenariosIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task CenariosAPI_SupportsBothCurrentAndFuture()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var responseAtual = await client.GetAsync("/api/cenarios/atual?subcategoriaId=1");
            var responseFuturo = await client.GetAsync("/api/cenarios/futuro?subcategoriaId=1");

            // Assert
            Assert.True(responseAtual.IsSuccessStatusCode, "Cenário atual deve funcionar");
            Assert.True(responseFuturo.IsSuccessStatusCode, "Cenário futuro deve funcionar");
        }

        [Fact]
        public async Task SalvarCenarioAtual_WithValidData_ReturnsSuccess()
        {
            // Arrange -  USA IDs E STRINGS ÚNICOS
            var client = CreateClient();
            var uniqueSubcategoriaId = GenerateUniqueId();
            var uniqueJustificativa = GenerateUniqueString("Justificativa");

            var cenarios = new List<CenarioAtualDto>
            {
                new CenarioAtualDto
                {
                    SUBCATEGORIA = uniqueSubcategoriaId,
                    PRIOR_ATUAL = 1,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = uniqueJustificativa,
                    POLIT_ATUAL = GenerateUniqueString("Politica"),
                    PRAT_ATUAL = GenerateUniqueString("Pratica"),
                    FUNC_RESP = GenerateUniqueString("Responsavel"),
                    REF_INFO = GenerateUniqueString("Referencia"),
                    EVID_ATUAL = GenerateUniqueString("Evidencia"),
                    NOTAS = GenerateUniqueString("Notas"),
                    CONSIDERACOES = GenerateUniqueString("Consideracoes"),
                },
            };

            var json = JsonSerializer.Serialize(cenarios);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/cenarios/atual/salvar", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("sucesso", responseContent.ToLower());

            //  VALIDA QUE FOI SALVO NO BANCO EM MEMÓRIA
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SgsiDbContext>();
            var saved = await dbContext.CenariosAtual.FirstOrDefaultAsync(c =>
                c.SUBCATEGORIA == uniqueSubcategoriaId
            );
            Assert.NotNull(saved);
            Assert.Equal(uniqueJustificativa, saved.JUSTIFICATIVA);
        }

        [Fact]
        public async Task GetCenariosFormatados_ReturnsFormattedData()
        {
            // Arrange -  USA DADOS ÚNICOS
            var client = CreateClient();
            var uniqueSubcategoriaId = GenerateUniqueId();

            var cenario = new List<CenarioAtualDto>
            {
                new CenarioAtualDto
                {
                    SUBCATEGORIA = uniqueSubcategoriaId,
                    PRIOR_ATUAL = 1,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = GenerateUniqueString("TesteFormatacao"),
                },
            };

            var json = JsonSerializer.Serialize(cenario);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("/api/cenarios/atual/salvar", content);

            // Act
            var response = await client.GetAsync("/api/cenarios/atual/formatados");
            var responseContent = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("subcategoriaFormatada", responseContent);
        }
    }
}
