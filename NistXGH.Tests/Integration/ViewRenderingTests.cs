// Testes de Renderização de Views
// NistXGH.Tests/Integration/ViewRenderingTests.cs
using Xunit;

namespace NistXGH.Tests.Integration
{
    public class ViewRenderingTests : IntegrationTestBase
    {
        [Theory]
        [InlineData("/")]
        [InlineData("/Home/Consultas")]
        [InlineData("/Home/Precadastro")]
        [InlineData("/Home/Relatorios")]
        public async Task Views_ShouldRender_WithoutErrors(string url)
        {
            // Arrange
            var client = CreateClient();

            // Act
            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("<!DOCTYPE html>", content);
            Assert.Contains("<html", content);
            Assert.Contains("</html>", content);
        }

        [Fact]
        public async Task ConsultasView_LoadsJavaScriptModules()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/Home/Consultas");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("<script", content);
        }

        [Fact]
        public async Task Views_ShouldInclude_CSSStylesheets()
        {
            // Arrange
            var client = CreateClient();

            // Act
            var response = await client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Contains("<link", content);
            Assert.Contains("stylesheet", content);
        }
    }
}
