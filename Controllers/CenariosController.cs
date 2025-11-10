using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;
using NistXGH.Models.Dto;
using NistXGH.Services;
using System.Data;
using Oracle.ManagedDataAccess.Client;

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
                _logger.LogInformation("Buscando cenário atual MAIS RECENTE para subcategoria {SubcategoriaId}", subcategoriaId);

                // Buscar o MAIS RECENTE - usando VERSAO
                var cenario = await _context.Set<CenarioAtual>()
                    .Where(c => c.SUBCATEGORIA == subcategoriaId)
                    .OrderByDescending(c => c.VERSAO)
                    .FirstOrDefaultAsync();

                if (cenario == null)
                {
                    _logger.LogWarning("Nenhum cenário atual encontrado para subcategoria {SubcategoriaId}", subcategoriaId);
                    return Ok(new CenarioAtualDto
                    {
                        SUBCATEGORIA = subcategoriaId,
                        JUSTIFICATIVA = "Registro a ser preenchido",
                        PRIOR_ATUAL = 0,
                        STATUS_ATUAL = 0,
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
        public async Task<IActionResult> SalvarCenarioAtual([FromBody] CenarioAtualDto cenarioDto)
        {
            if (cenarioDto == null)
                return BadRequest("Dados inválidos.");

            try
            {
                // ✅ SEMPRE CRIAR NOVO REGISTRO
                var novoCenario = new CenarioAtual
                {
                    SUBCATEGORIA = cenarioDto.SUBCATEGORIA,
                    JUSTIFICATIVA = cenarioDto.JUSTIFICATIVA ?? "Registro atualizado via sistema NIST CSF",
                    PRIOR_ATUAL = cenarioDto.PRIOR_ATUAL,
                    STATUS_ATUAL = cenarioDto.STATUS_ATUAL,
                    POLIT_ATUAL = cenarioDto.POLIT_ATUAL ?? "Não informado",
                    PRAT_ATUAL = cenarioDto.PRAT_ATUAL ?? "Não informado",
                    FUNC_RESP = cenarioDto.FUNC_RESP ?? "Não informado",
                    REF_INFO = cenarioDto.REF_INFO ?? "Não informado",
                    EVID_ATUAL = cenarioDto.EVID_ATUAL ?? "Não informado",
                    NOTAS = cenarioDto.NOTAS ?? "Sem notas adicionais",
                    CONSIDERACOES = cenarioDto.CONSIDERACOES ?? "Sem considerações adicionais",
                    DATA_REGISTRO = DateTime.Now,
                    VERSAO = DateTime.Now.Ticks
                };

                _context.CenarioAtual.Add(novoCenario);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    sucesso = true,
                    mensagem = "Novo registro de Cenário Atual criado com sucesso!",
                    id = novoCenario.ID
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
                _logger.LogInformation("Buscando cenário futuro MAIS RECENTE para subcategoria {SubcategoriaId}", subcategoriaId);

                // Buscar o MAIS RECENTE - usando VERSAO
                var cenario = await _context.Set<CenarioFuturo>()
                    .Where(c => c.SUBCATEGORIA == subcategoriaId)
                    .OrderByDescending(c => c.VERSAO)
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
            if (lista == null || lista.Count == 0)
                return BadRequest("Nenhum dado enviado.");

            try
            {
                var resultados = new List<object>();

                foreach (var item in lista)
                {
                    // ✅ SEMPRE CRIAR NOVO REGISTRO
                    var novoCenario = new CenarioFuturo
                    {
                        SUBCATEGORIA = item.SUBCATEGORIA,
                        POLIT_ALVO = item.POLIT_ALVO,
                        PRAT_ALVO = item.PRAT_ALVO,
                        ARTEF_ALVO = item.ARTEF_ALVO,
                        FUNC_ALVO = item.FUNC_ALVO,
                        REF_INFO_ALVO = item.REF_INFO_ALVO,
                        PRIORIDADE_ALVO = item.PRIORIDADE_ALVO,
                        NIVEL_ALVO = item.NIVEL_ALVO,
                        DATA_REGISTRO = DateTime.Now,
                        VERSAO = DateTime.Now.Ticks
                    };

                    _context.CenarioFuturo.Add(novoCenario);
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
                var subcategoriaIds = await _context.CenarioFuturo
                    .Select(c => c.SUBCATEGORIA)
                    .Distinct()
                    .ToListAsync();

                var cenariosMaisRecentes = new List<CenarioFuturo>();

                foreach (var subcategoriaId in subcategoriaIds)
                {
                    var maisRecente = await _context.CenarioFuturo
                        .Where(c => c.SUBCATEGORIA == subcategoriaId)
                        .OrderByDescending(c => c.VERSAO)
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
                var historico = await _context.CenarioAtual
                    .Where(c => c.SUBCATEGORIA == subcategoriaId)
                    .OrderByDescending(c => c.VERSAO)
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
                var historico = await _context.CenarioFuturo
                    .Where(c => c.SUBCATEGORIA == subcategoriaId)
                    .OrderByDescending(c => c.VERSAO)
                    .ToListAsync();

                return Ok(historico);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao buscar histórico: {ex.Message}");
            }
        }
    }
}