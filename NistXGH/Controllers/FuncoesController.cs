using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;
using NistXGH.Models.Dto;

[ApiController]
[Route("api/[controller]")]
public class FuncoesController : ControllerBase
{
    private readonly SgsiDbContext _context;

    public FuncoesController(SgsiDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FuncaoDto>>> GetFuncoes()
    {
        try
        {
            var funcoes = await _context
                .Funcoes.OrderBy(f => f.CODIGO)
                .Select(f => new FuncaoDto
                {
                    Id = f.ID,
                    Codigo = f.CODIGO ?? string.Empty,
                    Nome = f.NOME ?? string.Empty,
                })
                .ToListAsync();

            return Ok(funcoes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FuncaoDto>> GetFuncao(int id)
    {
        try
        {
            var f = await _context.Funcoes.FirstOrDefaultAsync(x => x.ID == id);

            if (f == null)
                return NotFound();

            var funcaoDto = new FuncaoDto
            {
                Id = f.ID,
                Codigo = f.CODIGO ?? string.Empty,
                Nome = f.NOME ?? string.Empty,
            };

            return Ok(funcaoDto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }
}
