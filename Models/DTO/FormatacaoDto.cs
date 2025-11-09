// Models/Dto/FormatacaoDto.cs
namespace NistXGH.Models.Dto
{
    public class FormatacaoDto
    {
        public int Id { get; set; }
        public string CodigoFormatado { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? FuncaoCodigo { get; set; }
        public string? FuncaoNome { get; set; }
        public string? CategoriaCodigo { get; set; }
        public string? CategoriaNome { get; set; }
        public string? NumeroSubcategoria { get; set; }
        public int? FuncaoId { get; set; }
        public int? CategoriaId { get; set; }
    }
}