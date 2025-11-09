using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;
using NistXGH.Models.Dto;

namespace NistXGH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CenarioFuturoController : ControllerBase
    {
        private readonly SgsiDbContext _context;

        public CenarioFuturoController(SgsiDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int subcategoriaId)
        {
            try
            {
                var cenario = await _context.Set<CenarioFuturo>()
                    .FirstOrDefaultAsync(c => c.SUBCATEGORIA == subcategoriaId);

                return Ok(cenario ?? new CenarioFuturo());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
        
        // POST: api/CenarioFuturo/salvar
        [HttpPost("salvar")]
        public async Task<IActionResult> Salvar([FromBody] List<CenarioFuturoDto> lista)
        {
            if (lista == null || lista.Count == 0)
                return BadRequest("Nenhum dado enviado.");

            foreach (var item in lista)
            {
                // Verifica se já existe um registro para essa subcategoria
                var existente = await _context.Set<CenarioFuturo>()
                    .FirstOrDefaultAsync(c => c.SUBCATEGORIA == item.SUBCATEGORIA);

                if (existente != null)
                {
                    // Atualiza campos
                    existente.POLIT_ALVO = item.POLIT_ALVO;
                    existente.PRAT_ALVO = item.PRAT_ALVO;
                    existente.ARTEF_ALVO = item.ARTEF_ALVO;
                    existente.FUNC_ALVO = item.FUNC_ALVO;
                    existente.REF_INFO_ALVO = item.REF_INFO_ALVO;
                    existente.PRIORIDADE_ALVO = item.PRIORIDADE_ALVO;
                    existente.NIVEL_ALVO = item.NIVEL_ALVO;
                    existente.DATA_REGISTRO = DateTime.Now;
                    _context.Update(existente);
                }
                else
                {
                    // Cria novo registro
                    var novo = new CenarioFuturo
                    {
                        SUBCATEGORIA = item.SUBCATEGORIA,
                        POLIT_ALVO = item.POLIT_ALVO,
                        PRAT_ALVO = item.PRAT_ALVO,
                        ARTEF_ALVO = item.ARTEF_ALVO,
                        FUNC_ALVO = item.FUNC_ALVO,
                        REF_INFO_ALVO = item.REF_INFO_ALVO,
                        PRIORIDADE_ALVO = item.PRIORIDADE_ALVO,
                        NIVEL_ALVO = item.NIVEL_ALVO,
                        DATA_REGISTRO = DateTime.Now
                    };
                    _context.Add(novo);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { sucesso = true, mensagem = "Cenário Futuro salvo com sucesso!" });
        }
    }

}
