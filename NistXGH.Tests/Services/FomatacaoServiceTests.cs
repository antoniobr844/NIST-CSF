using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NistXGH.Models;
using NistXGH.Services;
using Xunit;

namespace NistXGH.Tests.Services
{
    public class FormatacaoServiceTests : TestBase
    {
        private readonly SgsiDbContext _context;
        private readonly FormatacaoService _service;
        private readonly Mock<ILogger<FormatacaoService>> _mockLogger;

        public FormatacaoServiceTests()
        {
            _context = CreateMockDbContext();
            _mockLogger = new Mock<ILogger<FormatacaoService>>();
            _service = new FormatacaoService(_context, _mockLogger.Object);

            // Adicionar dados específicos para teste de formatação
            SeedFormatacaoTestData();
        }

        private void SeedFormatacaoTestData()
        {
            if (!_context.Funcoes.Any(f => f.CODIGO == "GV"))
            {
                _context.Funcoes.Add(
                    new Funcoes
                    {
                        ID = 1,
                        CODIGO = "GV",
                        NOME = "Governança",
                    }
                );
            }

            if (!_context.Categorias.Any(c => c.CODIGO == "AC"))
            {
                _context.Categorias.Add(
                    new Categorias
                    {
                        ID = 1,
                        CODIGO = "AC",
                        NOME = "Avaliação",
                        FUNCAO = 1,
                    }
                );
            }

            if (!_context.Subcategorias.Any(s => s.ID == 1))
            {
                _context.Subcategorias.Add(
                    new Subcategorias
                    {
                        ID = 1,
                        CATEGORIA = 1,
                        FUNCAO = 1,
                        SUBCATEGORIA = 1,
                        DESCRICAO = "Descrição teste",
                    }
                );
            }

            _context.SaveChanges();
        }

        [Fact]
        public async Task FormatSubcategoria_WithValidId_ReturnsFormattedString()
        {
            // Act
            var result = await _service.FormatSubcategoria(1);

            // Assert - Verificar que retorna alguma string formatada
            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public async Task FormatSubcategoria_WithInvalidId_ReturnsDefault()
        {
            // Act
            var result = await _service.FormatSubcategoria(9999);

            // Assert - Deve retornar "N/A" ou string padrão
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetSubcategoriaFormatadaCompleta_WithValidId_ReturnsCompleteInfo()
        {
            // Act
            var result = await _service.GetSubcategoriaFormatadaCompleta(1);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.CodigoFormatado);
        }

        [Fact]
        public async Task FormatSubcategorias_WithValidIds_ReturnsFormattedDictionary()
        {
            // Arrange
            var ids = new[] { 1 };

            // Act
            var result = await _service.FormatSubcategorias(ids);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetSubcategoriasFormatadasCompletas_WithValidIds_ReturnsCompleteDictionary()
        {
            // Arrange
            var ids = new[] { 1 };

            // Act
            var result = await _service.GetSubcategoriasFormatadasCompletas(ids);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }
    }
}
