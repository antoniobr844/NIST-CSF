using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;
using NistXGH.Models.Dto;
using NistXGH.Services;

namespace NistXGH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CenariosController : ControllerBase
    {
        private readonly SgsiDbContext _context;
        private readonly ILogger<CenariosController> _logger;
        private readonly IFormatacaoService _formatacaoService;

        public CenariosController(SgsiDbContext context, ILogger<CenariosController> logger, IFormatacaoService formatacaoService)
        {
            _context = context;
            _formatacaoService = formatacaoService;
            _logger = logger;
        }

        // ========== CENÁRIO ATUAL ==========

        [HttpGet("atual")]
        public async Task<IActionResult> GetCenarioAtual([FromQuery] int subcategoriaId)
        {
            try
            {
                _logger.LogInformation("Buscando cenário atual para subcategoria {SubcategoriaId}", subcategoriaId);

                // Buscar o MAIS RECENTE - usando DATA_REGISTRO
                var cenario = await _context.CenariosAtual
                    .Where(c => c.SUBCATEGORIA == subcategoriaId)
                    .OrderByDescending(c => c.DATA_REGISTRO)
                    .FirstOrDefaultAsync();

                if (cenario == null)
                {
                    _logger.LogWarning("Nenhum cenário atual encontrado para subcategoria {SubcategoriaId}", subcategoriaId);
                    return Ok(new CenarioAtualDto
                    {
                        SUBCATEGORIA = subcategoriaId,
                        JUSTIFICATIVA = "Registro a ser preenchido",
                        PRIOR_ATUAL = 0,
                        STATUS_ATUAL = "",
                        POLIT_ATUAL = "Não informado",
                        PRAT_ATUAL = "Não informado",
                        FUNC_RESP = "Não informado",
                        REF_INFO = "Não informado",
                        EVID_ATUAL = "Não informado",
                        NOTAS = "",
                        CONSIDERACOES = ""
                    });
                }

                _logger.LogInformation("Cenário atual encontrado para subcategoria {SubcategoriaId}", subcategoriaId);
                return Ok(cenario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar cenário atual para subcategoria {SubcategoriaId}", subcategoriaId);
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPost("atual/salvar")]
        // 🚨 CORREÇÃO: Receber uma lista de DTOs em vez de um único objeto
        public async Task<IActionResult> SalvarCenarioAtual([FromBody] List<CenarioAtualDto> lista)
        {
            if (lista == null || lista.Count == 0)
                return BadRequest("Nenhum dado enviado.");

            try
            {
                var resultados = new List<object>();

                foreach (var cenarioDto in lista)
                {
                    // ✅ SEMPRE CRIAR NOVO REGISTRO
                    var novoCenario = new CenarioAtual
                    {
                        SUBCATEGORIA = CenarioAtualDto.SUBCATEGORIA,
                        // Ajuste para permitir NULL no DB se o campo for opcional (conforme correção anterior)
                        JUSTIFICATIVA = CenarioAtualDto.PRIOR_ATUAL,
                        STATUS_ATUAL = CenarioAtualDto.STATUS_ATUAL,
                        POLIT_ATUAL = string.IsNullOrEmpty(cenarioDto.POLIT_ATUAL) ? null : cenarioDto.POLIT_ATUAL,
                        PRAT_ATUAL = string.IsNullOrEmpty(cenarioDto.PRAT_ATUAL) ? null : cenarioDto.PRAT_ATUAL,
                        FUNC_RESP = string.IsNullOrEmpty(cenarioDto.FUNC_RESP) ? null : cenarioDto.FUNC_RESP,
                        REF_INFO = string.IsNullOrEmpty(cenarioDto.REF_INFO) ? null : cenarioDto.REF_INFO,
                        EVID_ATUAL = string.IsNullOrEmpty(cenarioDto.EVID_ATUAL) ? null : cenarioDto.EVID_ATUAL,
                        NOTAS = string.IsNullOrEmpty(cenarioDto.NOTAS) ? null : cenarioDto.NOTAS,
                        CONSIDERACOES = string.IsNullOrEmpty(cenarioDto.CONSIDERACOES) ? null : cenarioDto.CONSIDERACOES,
                        DATA_REGISTRO = DateTime.Now
                    };

                    _context.CenariosAtual.Add(novoCenario);
                    resultados.Add(new
                    {
                        subcategoriaId = novoCenario.SUBCATEGORIA,
                        id = novoCenario.ID
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    sucesso = true,
                    mensagem = $"Novos registros de Cenário Atual ({lista.Count}) criados com sucesso!",
                    registros = resultados
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar cenário atual");
                return StatusCode(500, $"Erro ao salvar: {ex.Message}");
            }
        }

        // ========== CENÁRIO FUTURO ==========

        [HttpGet("futuro")]
        public async Task<IActionResult> GetCenarioFuturo([FromQuery] int subcategoriaId)
        {
            try
            {
                _logger.LogInformation("Buscando cenário futuro para subcategoria {SubcategoriaId}", subcategoriaId);

                // Buscar o MAIS RECENTE - usando DATA_REGISTRO
                var cenario = await _context.CenariosFuturo
                    .Where(c => c.SUBCATEGORIA == subcategoriaId)
                    .OrderByDescending(c => c.DATA_REGISTRO)
                    .FirstOrDefaultAsync();

                if (cenario == null)
                {
                    _logger.LogWarning("Nenhum cenário futuro encontrado para subcategoria {SubcategoriaId}", subcategoriaId);
                    return Ok(new CenarioFuturoDto
                    {
                        SUBCATEGORIA = subcategoriaId,
                        PRIORIDADE_ALVO = null,
                        NIVEL_ALVO = null,
                        POLIT_ALVO = "",
                        PRAT_ALVO = "",
                        FUNC_ALVO = "",
                        REF_INFO_ALVO = "",
                        ARTEF_ALVO = ""
                    });
                }

                return Ok(cenario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao buscar cenário futuro para subcategoria {SubcategoriaId}", subcategoriaId);
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        [HttpPost("futuro/salvar")]
        public async Task<IActionResult> SalvarCenarioFuturo([FromBody] List<CenarioFuturoDto> lista)
        {
            try
            {
                if (lista == null || lista.Count == 0)
                    return BadRequest("Nenhum dado enviado.");

                var resultados = new List<object>();

                foreach (var item in lista)
                {
                    // ✅ SEMPRE CRIAR NOVO REGISTRO
                    var novoCenario = new CenarioFuturo
                    {
                        SUBCATEGORIA = item.SUBCATEGORIA,
                        PRIORIDADE_ALVO = item.PRIORIDADE_ALVO,
                        NIVEL_ALVO = item.NIVEL_ALVO,
                        // Conversão para NULL se for string vazia para as colunas CLOB/String
                        POLIT_ALVO = string.IsNullOrEmpty(item.POLIT_ALVO) ? null : item.POLIT_ALVO,
                        PRAT_ALVO = string.IsNullOrEmpty(item.PRAT_ALVO) ? null : item.PRAT_ALVO,
                        ARTEF_ALVO = string.IsNullOrEmpty(item.ARTEF_ALVO) ? null : item.ARTEF_ALVO,
                        FUNC_ALVO = string.IsNullOrEmpty(item.FUNC_ALVO) ? null : item.FUNC_ALVO,
                        REF_INFO_ALVO = string.IsNullOrEmpty(item.REF_INFO_ALVO) ? null : item.REF_INFO_ALVO,
                        DATA_REGISTRO = DateTime.Now
                    };

                    _context.CenariosFuturo.Add(novoCenario);
                    resultados.Add(new
                    {
                        subcategoriaId = item.SUBCATEGORIA,
                        id = novoCenario.ID
                    });
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    sucesso = true,
                    mensagem = $"{lista.Count} novos registros de Cenário Futuro criados com sucesso!",
                    registros = resultados
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar cenário futuro");
                return StatusCode(500, $"Erro ao salvar: {ex.Message}");
            }
        }

        // ========== RELATÓRIOS FORMATADOS ==========

        [HttpGet("futuro/formatados")]
        public async Task<ActionResult> GetCenariosFuturoFormatados()
        {
            try
            {
                _logger.LogInformation("Buscando cenários futuros formatados...");

                // Busca apenas os MAIS RECENTES de cada subcategoria
                var subcategoriaIds = await _context.CenariosFuturo
                    .Select(c => c.SUBCATEGORIA)
                    .Distinct()
                    .ToListAsync();

                var cenariosMaisRecentes = new List<CenarioFuturo>();

                foreach (var subcategoriaId in subcategoriaIds)
                {
                    var maisRecente = await _context.CenariosFuturo
                        .Where(c => c.SUBCATEGORIA == subcategoriaId)
                        .OrderByDescending(c => c.DATA_REGISTRO)
                        .FirstOrDefaultAsync();

                    if (maisRecente != null)
                    {
                        cenariosMaisRecentes.Add(maisRecente);
                    }
                }

                _logger.LogInformation($"Encontrados {cenariosMaisRecentes.Count} cenários futuros mais recentes");

                if (!cenariosMaisRecentes.Any())
                {
                    return Ok(new List<object>());
                }

                // Formata as subcategorias
                Dictionary<int, FormatacaoDto> subcategoriasFormatadas;

                try
                {
                    var subcategoriaIdsFormatar = cenariosMaisRecentes.Select(c => c.SUBCATEGORIA).Distinct();
                    subcategoriasFormatadas = await _formatacaoService.GetSubcategoriasFormatadasCompletas(subcategoriaIdsFormatar);
                }
                catch (Exception formatacaoEx)
                {
                    _logger.LogError(formatacaoEx, "Falha crítica no serviço de formatação");

                    // Fallback: retorna dados básicos sem formatação
                    var resultadoFallback = cenariosMaisRecentes.Select(c => new
                    {
                        id = c.ID,
                        subcategoriaFormatada = $"ID:{c.SUBCATEGORIA}",
                        descricaoSubcategoria = "Erro ao carregar descrição",
                        prioridade = c.PRIORIDADE_ALVO,
                        nivel = c.NIVEL_ALVO,
                        politica = c.POLIT_ALVO,
                        pratica = c.PRAT_ALVO,
                        artefato = c.ARTEF_ALVO,
                        dataRegistro = c.DATA_REGISTRO,
                        subcategoriaId = c.SUBCATEGORIA
                    }).ToList();

                    return Ok(resultadoFallback);
                }

                // Monta resultado final
                var resultado = cenariosMaisRecentes.Select(c =>
                {
                    var subcategoriaInfo = subcategoriasFormatadas.ContainsKey(c.SUBCATEGORIA)
                        ? subcategoriasFormatadas[c.SUBCATEGORIA]
                        : null;

                    return new
                    {
                        id = c.ID,
                        subcategoriaFormatada = subcategoriaInfo?.CodigoFormatado ?? $"ID:{c.SUBCATEGORIA}",
                        descricaoSubcategoria = subcategoriaInfo?.Descricao ?? "Descrição não disponível",
                        prioridade = c.PRIORIDADE_ALVO,
                        nivel = c.NIVEL_ALVO,
                        politica = c.POLIT_ALVO,
                        pratica = c.PRAT_ALVO,
                        artefato = c.ARTEF_ALVO,
                        dataRegistro = c.DATA_REGISTRO,
                        subcategoriaId = c.SUBCATEGORIA,
                        funcaoCodigo = subcategoriaInfo?.FuncaoCodigo,
                        categoriaCodigo = subcategoriaInfo?.CategoriaCodigo
                    };
                }).ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro crítico ao buscar cenários futuros formatados");
                return StatusCode(500, new
                {
                    error = "Erro interno no servidor",
                    details = ex.Message
                });
            }
        }

        // ========== MÉTODOS AUXILIARES ==========

        [HttpGet("atual/historico/{subcategoriaId}")]
        public async Task<IActionResult> GetHistoricoAtual(int subcategoriaId)
        {
            try
            {
                var historico = await _context.CenariosAtual
                    .Where(c => c.SUBCATEGORIA == subcategoriaId)
                    .OrderByDescending(c => c.DATA_REGISTRO)
                    .ToListAsync();

                return Ok(historico);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar histórico: {ex.Message}");
            }
        }

        [HttpGet("futuro/historico/{subcategoriaId}")]
        public async Task<IActionResult> GetHistoricoFuturo(int subcategoriaId)
        {
            try
            {
                var historico = await _context.CenariosFuturo
                    .Where(c => c.SUBCATEGORIA == subcategoriaId)
                    .OrderByDescending(c => c.DATA_REGISTRO)
                    .ToListAsync();

                return Ok(historico);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar histórico: {ex.Message}");
            }

        }
        // ========== IMPLEMENTAÇÃO FUTURA ==========

        /* [HttpGet("atual/formatados")]
          public async Task<ActionResult> GetCenariosAtualFormatados()
          {
              // Futuro: similar ao futuro/formatados
          }

          // ========== CONSULTAS ESPECIAIS ==========

          [HttpGet("comparativo")]
          public async Task<ActionResult> GetComparativo([FromQuery] int subcategoriaId)
          {
              // Futuro: retorna atual + futuro para comparação
          }

          [HttpGet("resumo-por-funcao")]
          public async Task<ActionResult> GetResumoPorFuncao()
          {
              // Futuro: agrupado por função
          }*/
    }
}