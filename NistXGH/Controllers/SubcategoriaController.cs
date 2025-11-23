using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;

[ApiController]
[Route("api/[controller]")]
public class SubcategoriasController : ControllerBase
{
    private readonly SgsiDbContext _context;

    public SubcategoriasController(SgsiDbContext context)
    {
        _context = context;
    }

    // GET: api/Subcategories
    // GET: api/Subcategories?categoriaId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subcategorias>>> GetSubcategorias(
        [FromQuery] int? categoriaId
    )
    {
        try
        {
            var query = _context.Subcategorias.AsQueryable();

            if (categoriaId.HasValue)
            {
                query = query.Where(s => s.CATEGORIA == categoriaId.Value);
            }

            var subcategorias = await query.OrderBy(s => s.ID).ToListAsync();

            return Ok(subcategorias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    // GET: api/Subcategories/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Subcategorias>> GetSubcategoria(int id)
    {
        try
        {
            var subcategoria = await _context.Subcategorias.FirstOrDefaultAsync(s => s.ID == id);

            if (subcategoria == null)
                return NotFound();

            return Ok(subcategoria);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    // No SubcategoriasController.cs
    [HttpGet("com-codigos")]
    public async Task<ActionResult<IEnumerable<object>>> GetSubcategoriasComCodigos()
    {
        try
        {
            var subcategorias = await (
                from sub in _context.Subcategorias
                join funcao in _context.Funcoes on sub.FUNCAO equals funcao.ID
                join categoria in _context.Categorias
                    on new { CategoriaId = sub.CATEGORIA, FuncaoId = sub.FUNCAO } equals new
                    {
                        CategoriaId = categoria.ID,
                        FuncaoId = categoria.FUNCAO,
                    }
                select new
                {
                    id = sub.ID,
                    codigoFuncao = funcao.CODIGO,
                    codigoCategoria = categoria.CODIGO,
                    codigoSubcategoria = sub.SUBCATEGORIA,
                    descricao = sub.DESCRICAO,
                }
            ).ToListAsync();

            return Ok(subcategorias);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { erro = ex.Message });
        }
    }
}
