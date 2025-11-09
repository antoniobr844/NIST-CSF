using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;
using NistXGH.Models.Dto;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly SgsiDbContext _context;

    public CategoriasController(SgsiDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias([FromQuery] int? funcaoId)
    {
        try
        {
            var query = _context.Categorias.AsQueryable();

            if (funcaoId.HasValue)
                query = query.Where(c => c.FUNCAO == funcaoId.Value);

            var categorias = await query
                .OrderBy(c => c.CODIGO)
                .Select(c => new CategoriaDto
                {
                    Id = c.ID,
                    Codigo = c.CODIGO,
                    Nome = c.NOME, // ✅ usa a coluna correta
                    Funcao = c.FUNCAO
                })
                .ToListAsync();

            return Ok(categorias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        var c = await _context.Categorias.FirstOrDefaultAsync(x => x.ID == id);

        if (c == null)
            return NotFound();

        return new CategoriaDto
        {
            Id = c.ID,
            Codigo = c.CODIGO,
            Nome = c.NOME, // ✅ corrigido
            Funcao = c.FUNCAO
        };
    }
}
