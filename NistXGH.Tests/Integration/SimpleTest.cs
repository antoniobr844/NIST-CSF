using System.Text.Json;
using Xunit;

namespace NistXGH.Tests.Integration
{
    public class SimpleTest : IntegrationTestBase
    {
        [Fact]
        public async Task BasicTest_ShouldWork()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/api/funcoes");

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var funcoes = JsonSerializer.Deserialize<JsonElement>(content);

            Assert.True(funcoes.ValueKind == JsonValueKind.Array);
            Assert.Equal(2, funcoes.GetArrayLength());
        }

        [Fact]
        public async Task HomePage_ShouldLoad()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("<!DOCTYPE html>", content);
        }
    }
}
