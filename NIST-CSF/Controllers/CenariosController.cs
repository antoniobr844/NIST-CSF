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
                var cenario = await _context.CenariosAtual
                    .Where(c => c.SUBCATEGORIA == subcategoriaId)
                    .OrderByDescending(c => c.DATA_REGISTRO)
                    .FirstOrDefaultAsync();

                if (cenario == null)
                {
                    _logger.LogWarning("Nenhum cenário atual encontrado para subcategoria {SubcategoriaId}", subcategoriaId);
                    return Ok(new
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
                return StatusCode(500, new
                {
                    error = $"Erro interno: {ex.Message}",
                    details = ex.InnerException?.Message
                });
            }
        }

        [HttpPost("atual/salvar")]
        public async Task<IActionResult> SalvarCenarioAtual([FromBody] List<CenarioAtualDto> lista)
        {
            if (lista == null || lista.Count == 0)
                return BadRequest("Nenhum dado enviado.");

            try
            {
                _logger.LogInformation("Iniciando salvamento de {Count} registros de cenário atual", lista.Count);

                var novosCenarios = new List<CenarioAtual>();
                var registrosComErro = new List<object>();
                var registrosComSucesso = new List<object>();

                foreach (var cenarioDto in lista)
                {
                    try
                    {
                        if (cenarioDto.SUBCATEGORIA <= 0)
                        {
                            _logger.LogWarning("SUBCATEGORIA inválido: {Subcategoria}", cenarioDto.SUBCATEGORIA);
                            registrosComErro.Add(new
                            {
                                subcategoria = cenarioDto.SUBCATEGORIA,
                                erro = "SUBCATEGORIA inválida"
                            });
                            continue;
                        }

                        // ✅ CORREÇÃO: Validar e converter valores numéricos
                        var prioridade = cenarioDto.PRIOR_ATUAL;
                        var status = cenarioDto.STATUS_ATUAL;

                        // Validação adicional para números
                        if (prioridade < 0) prioridade = 1;
                        if (status < 0) status = 1;

                        var novoCenario = new CenarioAtual
                        {
                            SUBCATEGORIA = cenarioDto.SUBCATEGORIA,
                            JUSTIFICATIVA = cenarioDto.JUSTIFICATIVA,
                            PRIOR_ATUAL = prioridade,
                            STATUS_ATUAL = status,
                            POLIT_ATUAL = cenarioDto.POLIT_ATUAL,
                            PRAT_ATUAL = cenarioDto.PRAT_ATUAL,
                            FUNC_RESP = cenarioDto.FUNC_RESP,
                            REF_INFO = cenarioDto.REF_INFO,
                            EVID_ATUAL = cenarioDto.EVID_ATUAL,
                            NOTAS = cenarioDto.NOTAS,
                            CONSIDERACOES = cenarioDto.CONSIDERACOES,
                            DATA_REGISTRO = DateTime.Now
                        };

                        novosCenarios.Add(novoCenario);
                        registrosComSucesso.Add(new
                        {
                            subcategoria = cenarioDto.SUBCATEGORIA,
                            prioridade = prioridade,
                            status = status
                        });

                        _logger.LogInformation("Criado registro para subcategoria {Subcategoria} - Prioridade: {Prioridade}, Status: {Status}",
                            cenarioDto.SUBCATEGORIA, prioridade, status);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar subcategoria {Subcategoria}", cenarioDto.SUBCATEGORIA);
                        registrosComErro.Add(new
                        {
                            subcategoria = cenarioDto.SUBCATEGORIA,
                            erro = ex.Message
                        });
                    }
                }

                if (novosCenarios.Count == 0)
                {
                    return BadRequest(new
                    {
                        sucesso = false,
                        mensagem = "Nenhum registro válido para salvar.",
                        erros = registrosComErro
                    });
                }

                _context.CenariosAtual.AddRange(novosCenarios);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Salvos {Count} registros de cenário atual com sucesso", novosCenarios.Count);

                return Ok(new
                {
                    sucesso = true,
                    mensagem = $"Novos registros de Cenário Atual ({novosCenarios.Count}) criados com sucesso!",
                    registrosSalvos = registrosComSucesso,
                    erros = registrosComErro.Count > 0 ? registrosComErro : null,
                    totalProcessado = lista.Count,
                    totalSalvo = novosCenarios.Count,
                    totalErros = registrosComErro.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar cenário atual");
                return StatusCode(500, new
                {
                    error = $"Erro ao salvar: {ex.Message}",
                    innerException = ex.InnerException?.Message,
                    details = "Verifique os logs para mais informações"
                });
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
                    // SEMPRE CRIAR NOVO REGISTRO
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

        // ========== EDIÇÃO DE CENÁRIOS ==========

        [HttpPost("atual/editar")]
        public async Task<IActionResult> EditarCenarioAtual([FromBody] CenarioAtualDto dados)
        {
            try
            {
                _logger.LogInformation("Iniciando edição do cenário atual - ID: {Id}", dados.ID);

                var registro = await _context.CenariosAtual.FindAsync(dados.ID);
                if (registro == null)
                {
                    _logger.LogWarning("Cenário atual não encontrado para edição - ID: {Id}", dados.ID);
                    return NotFound("Registro não encontrado");
                }

                // Gerar log das alterações
                var alteracoes = await GerarLogAlteracoes(registro, dados, "ATUAL");

                // Aplicar alterações
                registro.PRIOR_ATUAL = dados.PRIOR_ATUAL;
                registro.STATUS_ATUAL = dados.STATUS_ATUAL;
                registro.POLIT_ATUAL = dados.POLIT_ATUAL;
                registro.PRAT_ATUAL = dados.PRAT_ATUAL;
                registro.FUNC_RESP = dados.FUNC_RESP;
                registro.REF_INFO = dados.REF_INFO;
                registro.EVID_ATUAL = dados.EVID_ATUAL;
                registro.JUSTIFICATIVA = dados.JUSTIFICATIVA;
                registro.NOTAS = dados.NOTAS;
                registro.CONSIDERACOES = dados.CONSIDERACOES;
                registro.DATA_REGISTRO = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cenário atual editado com sucesso - ID: {Id}, Alterações: {Count}",
                    dados.ID, alteracoes.Count);

                return Ok(new
                {
                    success = true,
                    message = "Registro atualizado com sucesso",
                    alteracoes = alteracoes.Count,
                    id = dados.ID
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar cenário atual - ID: {Id}", dados.ID);
                return StatusCode(500, $"Erro ao editar registro: {ex.Message}");
            }
        }

        [HttpPost("futuro/editar")]
        public async Task<IActionResult> EditarCenarioFuturo([FromBody] CenarioFuturoDto dados)
        {
            try
            {
                _logger.LogInformation("Iniciando edição do cenário futuro - ID: {Id}", dados.ID);

                var registro = await _context.CenariosFuturo.FindAsync(dados.ID);
                if (registro == null)
                {
                    _logger.LogWarning("Cenário futuro não encontrado para edição - ID: {Id}", dados.ID);
                    return NotFound("Registro não encontrado");
                }

                // Gerar log das alterações
                var alteracoes = await GerarLogAlteracoes(registro, dados, "FUTURO");

                // Aplicar alterações
                registro.PRIORIDADE_ALVO = dados.PRIORIDADE_ALVO;
                registro.NIVEL_ALVO = dados.NIVEL_ALVO;
                registro.POLIT_ALVO = dados.POLIT_ALVO;
                registro.PRAT_ALVO = dados.PRAT_ALVO;
                registro.FUNC_ALVO = dados.FUNC_ALVO;
                registro.REF_INFO_ALVO = dados.REF_INFO_ALVO;
                registro.ARTEF_ALVO = dados.ARTEF_ALVO;
                registro.DATA_REGISTRO = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Cenário futuro editado com sucesso - ID: {Id}, Alterações: {Count}",
                    dados.ID, alteracoes.Count);

                return Ok(new
                {
                    success = true,
                    message = "Registro atualizado com sucesso",
                    alteracoes = alteracoes.Count,
                    id = dados.ID
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar cenário futuro - ID: {Id}", dados.ID);
                return StatusCode(500, $"Erro ao editar registro: {ex.Message}");
            }
        }

        // ========== MÉTODO AUXILIAR PARA LOG ==========

        private async Task<List<LogAlteracao>> GerarLogAlteracoes(object registroAntigo, object registroNovo, string tipoCenario)
        {
            var alteracoes = new List<LogAlteracao>();
            var propriedades = registroAntigo.GetType().GetProperties();

            foreach (var prop in propriedades)
            {
                // Ignorar propriedades que não devem ser logadas
                if (prop.Name == "ID" || prop.Name == "DATA_REGISTRO" || prop.Name == "SUBCATEGORIA")
                    continue;

                var valorAntigo = prop.GetValue(registroAntigo)?.ToString();
                var valorNovo = prop.GetValue(registroNovo)?.ToString();

                if (valorAntigo != valorNovo)
                {
                    var log = new CenarioLog
                    {
                        CENARIO_ID = ((dynamic)registroAntigo).ID,
                        CENARIO_TIPO = tipoCenario,
                        SUBCATEGORIA_ID = ((dynamic)registroAntigo).SUBCATEGORIA,
                        CAMPO_ALTERADO = prop.Name,
                        VALOR_ANTIGO = valorAntigo,
                        VALOR_NOVO = valorNovo,
                        USUARIO = User.Identity?.Name ?? "Sistema",
                        DATA_ALTERACAO = DateTime.Now,
                        IP_MAQUINA = HttpContext.Connection.RemoteIpAddress?.ToString()
                    };

                    _context.CenarioLog.Add(log);
                    alteracoes.Add(new LogAlteracao
                    {
                        Campo = prop.Name,
                        ValorAntigo = valorAntigo,
                        ValorNovo = valorNovo
                    });

                    _logger.LogInformation("Campo alterado: {Campo} - De: {ValorAntigo} Para: {ValorNovo}",
                        prop.Name, valorAntigo, valorNovo);
                }
            }

            if (alteracoes.Any())
                await _context.SaveChangesAsync();

            return alteracoes;
        }

 
        // Classe auxiliar para retorno do log


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