// Services/FormatacaoSubcategoriaService.cs CORRIGIDO
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;
using NistXGH.Models.Dto;

namespace NistXGH.Services
{
    public class FormatacaoService : IFormatacaoService
    {
        private readonly SgsiDbContext _context;
        private readonly ILogger<FormatacaoService> _logger;

        public FormatacaoService(
            SgsiDbContext context,
            ILogger<FormatacaoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> FormatSubcategoria(int subcategoriaId)
        {
            try
            {
                var subcategoria = await CarregarSubcategoriaComRelacionamentos(subcategoriaId);
                return FormatSubcategoriaInterno(subcategoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao formatar subcategoria {SubcategoriaId}", subcategoriaId);
                return $"ERRO:{subcategoriaId}";
            }
        }

        public async Task<Dictionary<int, string>> FormatSubcategorias(IEnumerable<int> subcategoriaIds)
        {
            try
            {
                var idsDistintos = subcategoriaIds.Distinct().ToList();

                var subcategorias = await _context.Subcategorias
                    .Include(s => s.CategoriaNav)
                        .ThenInclude(c => c.FuncaoNav) // ✅ CORREÇÃO: Acessa função através da categoria
                    .Where(s => idsDistintos.Contains(s.ID))
                    .AsNoTracking()
                    .ToListAsync();

                var resultado = subcategorias
                    .ToDictionary(
                        s => s.ID,
                        s => FormatSubcategoriaInterno(s)
                    );

                // Preencher com erro para IDs não encontrados
                foreach (var id in idsDistintos)
                {
                    if (!resultado.ContainsKey(id))
                    {
                        resultado[id] = $"NÃO_ENCONTRADO:{id}";
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao formatar subcategorias {SubcategoriaIds}", subcategoriaIds);
                return subcategoriaIds.ToDictionary(id => id, id => $"ERRO:{id}");
            }
        }

        public async Task<FormatacaoDto> GetSubcategoriaFormatadaCompleta(int subcategoriaId)
        {
            try
            {
                var subcategoria = await CarregarSubcategoriaComRelacionamentos(subcategoriaId);

                if (subcategoria == null)
                    return null;

                return MapearParaDto(subcategoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter subcategoria completa {SubcategoriaId}", subcategoriaId);
                return new FormatacaoDto
                {
                    Id = subcategoriaId,
                    CodigoFormatado = $"ERRO:{subcategoriaId}",
                    Descricao = "Erro ao carregar subcategoria"
                };
            }
        }

        public async Task<Dictionary<int, FormatacaoDto>> GetSubcategoriasFormatadasCompletas(IEnumerable<int> subcategoriaIds)
        {
            try
            {
                var idsDistintos = subcategoriaIds.Distinct().ToList();

                var subcategorias = await _context.Subcategorias
                    .Include(s => s.CategoriaNav)
                        .ThenInclude(c => c.FuncaoNav) // ✅ CORREÇÃO: Acessa função através da categoria
                    .Where(s => idsDistintos.Contains(s.ID))
                    .AsNoTracking()
                    .ToListAsync();

                var resultado = subcategorias
                    .ToDictionary(
                        s => s.ID,
                        s => MapearParaDto(s)
                    );

                // Preencher com DTO de erro para IDs não encontrados
                foreach (var id in idsDistintos)
                {
                    if (!resultado.ContainsKey(id))
                    {
                        resultado[id] = new FormatacaoDto
                        {
                            Id = id,
                            CodigoFormatado = $"NÃO_ENCONTRADO:{id}",
                            Descricao = "Subcategoria não encontrada"
                        };
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter subcategorias completas {SubcategoriaIds}", subcategoriaIds);

                return subcategoriaIds.ToDictionary(id => id, id => new FormatacaoDto
                {
                    Id = id,
                    CodigoFormatado = $"ERRO:{id}",
                    Descricao = "Erro ao carregar subcategoria"
                });
            }
        }

        private async Task<Subcategorias> CarregarSubcategoriaComRelacionamentos(int subcategoriaId)
        {
            return await _context.Subcategorias
                .Include(s => s.CategoriaNav)
                    .ThenInclude(c => c.FuncaoNav) // ✅ CORREÇÃO
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == subcategoriaId);
        }

        private string FormatSubcategoriaInterno(Subcategorias subcategoria)
        {
            if (subcategoria == null)
                return "N/A";

            // ✅ CORREÇÃO: A função vem através da categoria
            var codigoFuncao = subcategoria.CategoriaNav?.FuncaoNav?.CODIGO ?? "??";
            var codigoCategoria = subcategoria.CategoriaNav?.CODIGO ?? "??";
            var numeroSubcategoria = subcategoria.SUBCATEGORIA ?? "??";

            return $"{codigoFuncao}.{codigoCategoria}-{numeroSubcategoria}";
        }

        private FormatacaoDto MapearParaDto(Subcategorias subcategoria)
        {
            // ✅ CORREÇÃO: A função vem através da categoria
            var categoria = subcategoria.CategoriaNav;
            var funcao = categoria?.FuncaoNav;

            return new FormatacaoDto
            {
                Id = subcategoria.ID,
                CodigoFormatado = FormatSubcategoriaInterno(subcategoria),
                Descricao = subcategoria.DESCRICAO ?? string.Empty,
                FuncaoCodigo = funcao?.CODIGO,
                FuncaoNome = funcao?.NOME,
                CategoriaCodigo = categoria?.CODIGO,
                CategoriaNome = categoria?.NOME,
                NumeroSubcategoria = subcategoria.SUBCATEGORIA,
                FuncaoId = funcao?.ID, // ✅ Agora vem da função através da categoria
                CategoriaId = categoria?.ID
            };
        }
    }
}