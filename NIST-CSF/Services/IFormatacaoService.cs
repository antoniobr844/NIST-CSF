// Services/IFormatacaoSubcategoriaService.cs
using NistXGH.Models.Dto;

namespace NistXGH.Services
{
    public interface IFormatacaoService
    {
        Task<string> FormatSubcategoria(int subcategoriaId);
        Task<Dictionary<int, string>> FormatSubcategorias(IEnumerable<int> subcategoriaIds);
        Task<FormatacaoDto> GetSubcategoriaFormatadaCompleta(int subcategoriaId);
        Task<Dictionary<int, FormatacaoDto>> GetSubcategoriasFormatadasCompletas(IEnumerable<int> subcategoriaIds);
    }
}