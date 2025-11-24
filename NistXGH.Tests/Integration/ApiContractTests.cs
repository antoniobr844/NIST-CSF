// Testes de Contrato API
using System.Text.Json;
using Xunit;

namespace NistXGH.Tests.Integration
{
    public class ApiContractTests : IntegrationTestBase
    {
        [Theory]
        [InlineData("/api/funcoes")]
        [InlineData("/api/categorias")]
        [InlineData("/api/subcategorias")]
        [InlineData("/api/cenarios/atual?subcategoriaId=1")]
        [InlineData("/api/cenarios/futuro?subcategoriaId=1")]
        public async Task API_Endpoints_ShouldReturnConsistentStructure(string endpoint)
        {
            // Arrange
            var client = CreateClient();

            // Act
            var response = await client.GetAsync(endpoint);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();

            // Verificar que retorna JSON válido
            var json = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.True(
                json.ValueKind == JsonValueKind.Array || json.ValueKind == JsonValueKind.Object,
                "Deve retornar array ou objeto JSON"
            );
        }

        [Fact]
        public async Task APIs_ShouldReturnJSONContentType()
        {
            // Arrange
            var client = CreateClient();
            var endpoints = new[] { "/api/funcoes", "/api/categorias", "/api/subcategorias" };

            foreach (var endpoint in endpoints)
            {
                // Act
                var response = await client.GetAsync(endpoint);

                // Assert
                response.EnsureSuccessStatusCode();
                Assert.Equal(
                    "application/json; charset=utf-8",
                    response.Content.Headers.ContentType?.ToString()
                );
            }
        }
    }
}
