// testes de integração frontend-backend
using System.Net;
using System.Text.Json;
using Xunit;

namespace NistXGH.Tests.Integration
{
    public class FrontendIntegrationTests : IntegrationTestBase
    {
        [Fact]
        public async Task JavaScriptModules_CanLoad_FromAPIs()
        {
            // Arrange
            var client = CreateClient();

            // Act - Simular requisições que o JavaScript faz
            var response = await client.GetAsync("/api/funcoes");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", 
                response.Content.Headers.ContentType?.ToString());
        }

        [Fact]
        public async Task FuncoesAPI_ReturnsExpectedDataStructure()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/api/funcoes");
            var content = await response.Content.ReadAsStringAsync();
            var funcoes = JsonSerializer.Deserialize<JsonElement>(content);

            // Assert - Verificar estrutura esperada pelo frontend
            response.EnsureSuccessStatusCode();
            Assert.True(funcoes.ValueKind == JsonValueKind.Array, "Deve retornar array");
            Assert.True(funcoes.GetArrayLength() > 0, "Array não deve estar vazio");

            // Verificar que contém dados esperados pelo JavaScript
            var funcoesString = content.ToLower();
            Assert.Contains("gov", funcoesString); // GV -> Governança
            Assert.Contains("id", funcoesString);  // IDs presentes
        }

        [Fact]
        public async Task CategoriasAPI_ReturnsFilteredData()
        {
            // Arrange
            var client = CreateClient();

            // Act - Testar filtro por função (como o frontend faz)
            var response = await client.GetAsync("/api/categorias?funcaoId=1");
            var content = await response.Content.ReadAsStringAsync();
            var categorias = JsonSerializer.Deserialize<JsonElement>(content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(categorias.ValueKind == JsonValueKind.Array);
            
            // Todas as categorias devem pertencer à função 1
            foreach (var categoria in categorias.EnumerateArray())
            {
                if (categoria.TryGetProperty("FUNCAO", out var funcaoProp))
                {
                    Assert.Equal(1, funcaoProp.GetInt32());
                }
            }
        }

        [Fact]
        public async Task SubcategoriasAPI_SupportsCategoriaFilter()
        {
            // Arrange
            var client = CreateClient();

            // Act - Testar filtro por categoria (como o frontend faz)
            var response = await client.GetAsync("/api/subcategorias?categoriaId=1");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("AC-1", content); // Dado do seed
        }
    }
}