using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;
using NistXGH.Models.Dto;

namespace NistXGH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CenarioAtualController : ControllerBase  // Mude para ControllerBase
    {
        private readonly SgsiDbContext _context;

        public CenarioAtualController(SgsiDbContext context)
        {
            _context = context;
        }

        // GET: api/CenarioAtual?subcategoriaId=5
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int subcategoriaId)
        {
            try
            {
                var cenario = await _context.Set<CenarioAtual>()
                    .FirstOrDefaultAsync(c => c.SUBCATEGORIA == subcategoriaId);

                return Ok(cenario ?? new CenarioAtual());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        // POST: api/CenarioAtual/salvar
        [HttpPost("salvar")]
        public async Task<IActionResult> Salvar([FromBody] CenarioAtualDto cenarioDto)
        {
            if (cenarioDto == null)
                return BadRequest("Dados inválidos.");

            try
            {
                // Verifica se já existe um registro para essa subcategoria
                var existente = await _context.Set<CenarioAtual>()
                    .FirstOrDefaultAsync(c => c.SUBCATEGORIA == cenarioDto.SUBCATEGORIA);

                if (existente != null)
                {
                    // Atualiza campos
                    existente.JUSTIFICATIVA = cenarioDto.JUSTIFICATIVA;
                    existente.PRIOR_ATUAL = cenarioDto.PRIOR_ATUAL;
                    existente.NIVEL_ATUAL = cenarioDto.NIVEL_ATUAL;
                    existente.POLIT_ATUAL = cenarioDto.POLIT_ATUAL;
                    existente.PRAT_ATUAL = cenarioDto.PRAT_ATUAL;
                    existente.FUNC_RESP = cenarioDto.FUNC_RESP;
                    existente.REF_INFO = cenarioDto.REF_INFO;
                    existente.EVID_ATUAL = cenarioDto.EVID_ATUAL;
                    existente.NOTAS = cenarioDto.NOTAS;
                    existente.CONSIDERACOES = cenarioDto.CONSIDERACOES;
                    existente.ICONE = cenarioDto.ICONE;
                    existente.DATA_REGISTRO = DateTime.Now;
                    _context.Update(existente);
                }
                else
                {
                    // Cria novo registro
                    var novo = new CenarioAtual
                    {
                        SUBCATEGORIA = cenarioDto.SUBCATEGORIA,
                        JUSTIFICATIVA = cenarioDto.JUSTIFICATIVA,
                        PRIOR_ATUAL = cenarioDto.PRIOR_ATUAL,
                        NIVEL_ATUAL = cenarioDto.NIVEL_ATUAL,
                        POLIT_ATUAL = cenarioDto.POLIT_ATUAL,
                        PRAT_ATUAL = cenarioDto.PRAT_ATUAL,
                        FUNC_RESP = cenarioDto.FUNC_RESP,
                        REF_INFO = cenarioDto.REF_INFO,
                        EVID_ATUAL = cenarioDto.EVID_ATUAL,
                        NOTAS = cenarioDto.NOTAS,
                        CONSIDERACOES = cenarioDto.CONSIDERACOES,
                        ICONE = cenarioDto.ICONE,
                        DATA_REGISTRO = DateTime.Now
                    };
                    _context.Add(novo);
                }

                await _context.SaveChangesAsync();
                return Ok(new { sucesso = true, mensagem = "Cenário Atual salvo com sucesso!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao salvar: {ex.Message}");
            }
        }
    }
}