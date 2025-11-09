using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models; // Adicione este using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NistXGH.Controllers // Adicione o namespace
{
    [ApiController]
    [Route("api/[controller]")]
    public class CenarioFuturoController : ControllerBase
    {
        private readonly SgsiDbContext _context;
        private readonly ILogger<CenarioFuturoController> _logger;

        public CenarioFuturoController(SgsiDbContext context, ILogger<CenarioFuturoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/CenarioFuturo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCenarioFuturo()
        {
            try
            {
                _logger.LogInformation("Buscando todos os cenários futuros");

                var cenarioFuturo = await _context.CenariosFuturos
                    .Include(c => c.Prioridade)
                    .Include(c => c.Nivel)
                    .Include(c => c.SubcategoriaNav)
                    .ThenInclude(s => s.FuncaoNav)
                    .Select(c => new
                    {
                        c.ID,
                        c.SUBCATEGORIA,
                        Prioridade = c.Prioridade != null ? c.Prioridade.NIVEL : null,
                        Nivel = c.Nivel != null ? c.Nivel.STATUS : null,
                        c.POLIT_ALVO,
                        c.PRAT_ALVO,
                        c.ARTEF_ALVO,
                        c.FUNC_ALVO,
                        c.REF_INFO_ALVO,
                        c.DATA_REGISTRO,
                        SubcategoriaInfo = c.SubcategoriaNav != null
                            ? new
                            {
                                CodigoFuncao = c.SubcategoriaNav.FuncaoNav != null
                                    ? c.SubcategoriaNav.FuncaoNav.CODIGO
                                    : null,
                                Categoria = c.SubcategoriaNav.CATEGORIA,
                                CodigoSubcategoria = c.SubcategoriaNav.SUBCATEGORIA,
                            }
                            : null,
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(cenarioFuturo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar cenários futuros");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // POST: api/CenarioFuturo
        [HttpPost]
        public async Task<ActionResult> SalvarCenarioFuturo([FromBody] List<CenarioFuturoDto> cenarios)
        {
            try
            {
                _logger.LogInformation($"Salvando {cenarios.Count} cenários futuros");

                foreach (var cenarioDto in cenarios)
                {
                    // Verificar se já existe
                    var existing = await _context.CenariosFuturos.FirstOrDefaultAsync(c =>
                        c.SUBCATEGORIA == cenarioDto.SubcategoriaId
                    );

                    if (existing != null)
                    {
                        // Atualizar existente
                        existing.PRIORIDADE_ALVO = cenarioDto.PrioridadeAlvo;
                        existing.NIVEL_ALVO = cenarioDto.NivelAlvo;
                        existing.POLIT_ALVO = cenarioDto.PoliticasAlvo;
                        existing.PRAT_ALVO = cenarioDto.PraticasAlvo;
                        existing.FUNC_ALVO = cenarioDto.FuncoesAlvo;
                        existing.REF_INFO_ALVO = cenarioDto.ReferenciasAlvo;
                        existing.ARTEF_ALVO = cenarioDto.ArtefatosAlvo;
                        existing.SUBCATEGORIA = cenarioDto.SubcategoriaId;
                        existing.DATA_REGISTRO = DateTime.Now;

                        _context.CenariosFuturos.Update(existing);
                    }
                    else
                    {
                        // Criar novo
                        var novoCenario = new CenarioFuturo // Mudei para CenarioFuturo
                        {
                            PRIORIDADE_ALVO = cenarioDto.PrioridadeAlvo,
                            NIVEL_ALVO = cenarioDto.NivelAlvo,
                            POLIT_ALVO = cenarioDto.PoliticasAlvo,
                            PRAT_ALVO = cenarioDto.PraticasAlvo,
                            FUNC_ALVO = cenarioDto.FuncoesAlvo,
                            REF_INFO_ALVO = cenarioDto.ReferenciasAlvo,
                            ARTEF_ALVO = cenarioDto.ArtefatosAlvo,
                            SUBCATEGORIA = cenarioDto.SubcategoriaId,
                            DATA_REGISTRO = DateTime.Now,
                        };

                        await _context.CenariosFuturos.AddAsync(novoCenario);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Cenários futuros salvos com sucesso");

                return Ok(new { message = "Dados salvos com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar cenários futuros");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/CenarioFuturo/com-subcategorias-formatadas
        [HttpGet("com-subcategorias-formatadas")]
        public async Task<ActionResult<IEnumerable<object>>> GetCenarioFuturoComSubcategoriasFormatadas()
        {
            try
            {
                var cenarios = await _context.CenariosFuturos
                    .Include(c => c.SubcategoriaNav)
                    .ThenInclude(s => s.FuncaoNav)
                    .Select(c => new
                    {
                        id = c.ID,
                        subcategoriaFormatada = c.SubcategoriaNav != null
                            ? $"{c.SubcategoriaNav.FuncaoNav.CODIGO}.{c.SubcategoriaNav.CATEGORIA}-{c.SubcategoriaNav.SUBCATEGORIA}"
                            : $"ID: {c.SUBCATEGORIA}",
                        prioridade = c.Prioridade != null ? c.Prioridade.NIVEL : null,
                        nivel = c.Nivel != null ? c.Nivel.STATUS : null,
                        politica = c.POLIT_ALVO,
                        pratica = c.PRAT_ALVO,
                        artefato = c.ARTEF_ALVO,
                        funcao = c.FUNC_ALVO,
                        referencia = c.REF_INFO_ALVO,
                        dataRegistro = c.DATA_REGISTRO,
                    })
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(cenarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DTO para receber os dados do frontend
        public class CenarioFuturoDto
        {
            public int? PrioridadeAlvo { get; set; }
            public int? NivelAlvo { get; set; }
            public string PoliticasAlvo { get; set; }
            public string PraticasAlvo { get; set; }
            public string FuncoesAlvo { get; set; }
            public string ReferenciasAlvo { get; set; }
            public string ArtefatosAlvo { get; set; }
            public int SubcategoriaId { get; set; }
            public DateTime DataRegistro { get; set; }
        }
    }
}