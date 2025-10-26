// Controllers/CategoriaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;

namespace NistXGH.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly SgsiDbContext _context;

        public CategoriasController(SgsiDbContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        // GET: api/Categorias?funcaoId=1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categorias>>> GetCategorias(
            [FromQuery] int? funcaoId
        )
        {
            try
            {
                var query = _context.Categorias.AsQueryable();

                if (funcaoId.HasValue)
                {
                    query = query.Where(c => c.FUNCAO == funcaoId.Value);
                }

                var categorias = await query.OrderBy(c => c.ID).ToListAsync();

                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }
    }
}
