using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;
using NistXGH.Models.Dto;

[ApiController]
[Route("api/[controller]")]
public class SubcategoriasController : ControllerBase
{
    private readonly SgsiDbContext _context;

    public SubcategoriasController(SgsiDbContext context)
    {
        _context = context;
    }

    // 🔹 GET: api/Subcategorias?categoriaId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubcategoriaDto>>> GetSubcategorias([FromQuery] int? categoriaId)
    {
        try
        {
            var query = _context.Subcategorias.AsQueryable();

            if (categoriaId.HasValue)
                query = query.Where(s => s.CATEGORIA == categoriaId.Value);

            var resultado = await query
                .OrderBy(s => s.SUBCATEGORIA)
                .Select(s => new SubcategoriaDto
                {
                    Id = s.ID,
                    Codigo = s.SUBCATEGORIA,
                    Nome = s.DESCRICAO,      // ✅ usa o campo de descrição como "Nome"
                    Descricao = s.DESCRICAO, // mantém compatibilidade
                    Categoria = s.CATEGORIA,
                    Funcao = s.FUNCAO
                })
                .ToListAsync();

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    // 🔹 GET: api/Subcategorias/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SubcategoriaDto>> GetSubcategoria(int id)
    {
        try
        {
            var s = await _context.Subcategorias.FirstOrDefaultAsync(s => s.ID == id);

            if (s == null)
                return NotFound();

            var dto = new SubcategoriaDto
            {
                Id = s.ID,
                Codigo = s.SUBCATEGORIA,
                Nome = s.DESCRICAO,
                Descricao = s.DESCRICAO,
                Categoria = s.CATEGORIA,
                Funcao = s.FUNCAO
            };

            return Ok(dto);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    // 🔹 GET: api/Subcategorias/com-codigos
    [HttpGet("com-codigos")]
    public async Task<ActionResult<IEnumerable<object>>> GetSubcategoriasComCodigos()
    {
        try
        {
            var subcategorias = await (
                from sub in _context.Subcategorias
                join funcao in _context.Funcoes on sub.FUNCAO equals funcao.ID
                join categoria in _context.Categorias
                    on new { CategoriaId = sub.CATEGORIA, FuncaoId = sub.FUNCAO }
                    equals new { CategoriaId = categoria.ID, FuncaoId = categoria.FUNCAO }
                orderby funcao.CODIGO, categoria.CODIGO, sub.SUBCATEGORIA
                select new
                {
                    id = sub.ID,
                    codigo = $"{funcao.CODIGO}.{categoria.CODIGO}.{sub.SUBCATEGORIA}",
                    nome = sub.DESCRICAO
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
