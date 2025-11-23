using System.ComponentModel.DataAnnotations;
using NistXGH.Models.Dto;
using Xunit;

namespace NistXGH.Tests.Models
{
    public class ModelValidationTests
    {
        [Theory]
        [InlineData(0, true)] 
        [InlineData(-1, true)] 
        [InlineData(1, true)] 
        public void CenarioAtualDto_ValidacaoSUBCATEGORIA(int subcategoria, bool esperadoValido)
        {
            // Arrange
            var dto = new CenarioAtualDto
            {
                SUBCATEGORIA = subcategoria,
                PRIOR_ATUAL = 1,
                STATUS_ATUAL = 1,
                JUSTIFICATIVA = "Teste válido",
            };

            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.Equal(esperadoValido, isValid);
        }

        [Fact]
        public void CenarioAtualDto_ComDadosCompletos_DeveSerValido()
        {
            // Arrange
            var dto = new CenarioAtualDto
            {
                SUBCATEGORIA = 1,
                PRIOR_ATUAL = 1,
                STATUS_ATUAL = 1,
                JUSTIFICATIVA = "Justificativa válida",
                POLIT_ATUAL = "Política atual",
                PRAT_ATUAL = "Prática atual",
                FUNC_RESP = "Responsável",
                REF_INFO = "Referência",
                EVID_ATUAL = "Evidência",
                NOTAS = "Notas adicionais",
                CONSIDERACOES = "Considerações finais",
            };

            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void FuncaoDto_PropriedadesObrigatorias_DevemSerValidas()
        {
            // Arrange & Act
            var dto = new FuncaoDto
            {
                Id = 1,
                Codigo = "GV",
                Nome = "Governança",
            };

            // Assert
            Assert.Equal(1, dto.Id);
            Assert.Equal("GV", dto.Codigo);
            Assert.Equal("Governança", dto.Nome);
        }

        [Theory]
        [InlineData("", true)] 
        [InlineData("GV", true)]
        public void FuncaoDto_ValidacaoCodigo(string codigo, bool esperadoValido)
        {
            // Arrange
            var dto = new FuncaoDto
            {
                Id = 1,
                Codigo = codigo,
                Nome = "Governança",
            };

            var context = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(dto, context, results, true);

            // Assert
            Assert.Equal(esperadoValido, isValid);
        }
    }
}
