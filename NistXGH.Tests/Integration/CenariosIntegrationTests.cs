// Testes de API para Cenários (Atual vs Futuro)
using System.Text;
using System.Text.Json;
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
            // Arrange
            var client = CreateClient();
            var cenarios = new List<CenarioAtualDto>
            {
                new CenarioAtualDto
                {
                    SUBCATEGORIA = 1,
                    PRIOR_ATUAL = 1,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = "Teste de integração",
                    POLIT_ATUAL = "Política teste",
                    PRAT_ATUAL = "Prática teste",
                    FUNC_RESP = "Responsável teste",
                    REF_INFO = "Referência teste",
                    EVID_ATUAL = "Evidência teste",
                    NOTAS = "Notas teste",
                    CONSIDERACOES = "Considerações teste",
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
        }

        [Fact]
        public async Task GetCenariosFormatados_ReturnsFormattedData()
        {
            // Arrange
            var client = CreateClient();

            // Primeiro criar um cenário para formatar
            var cenario = new List<CenarioAtualDto>
            {
                new CenarioAtualDto
                {
                    SUBCATEGORIA = 1,
                    PRIOR_ATUAL = 1,
                    STATUS_ATUAL = 1,
                    JUSTIFICATIVA = "Teste formatação",
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
