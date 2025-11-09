// Controllers/FormatacaoController.cs
using Microsoft.AspNetCore.Mvc;
using NistXGH.Services;
using NistXGH.Models.Dto;

namespace NistXGH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormatacaoController : ControllerBase
    {
        private readonly IFormatacaoService _formatacaoService;

        public FormatacaoController(IFormatacaoService formatacaoService)
        {
            _formatacaoService = formatacaoService;
        }

        [HttpGet("subcategoria/{id}")]
        public async Task<ActionResult<string>> GetSubcategoriaFormatada(int id)
        {
            try
            {
                var formatada = await _formatacaoService.FormatSubcategoria(id);
                return Ok(formatada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("subcategorias")]
        public async Task<ActionResult<Dictionary<int, string>>> GetSubcategoriasFormatadas([FromBody] int[] ids)
        {
            try
            {
                var formatadas = await _formatacaoService.FormatSubcategorias(ids);
                return Ok(formatadas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("subcategoria/completa/{id}")]
        public async Task<ActionResult<FormatacaoDto>> GetSubcategoriaCompleta(int id)
        {
            try
            {
                var completa = await _formatacaoService.GetSubcategoriaFormatadaCompleta(id);
                if (completa == null)
                    return NotFound(new { message = "Subcategoria não encontrada" });

                return Ok(completa);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("subcategorias/completas")]
        public async Task<ActionResult<Dictionary<int, FormatacaoDto>>> GetSubcategoriasCompletas([FromBody] int[] ids)
        {
            try
            {
                var completas = await _formatacaoService.GetSubcategoriasFormatadasCompletas(ids);
                return Ok(completas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}