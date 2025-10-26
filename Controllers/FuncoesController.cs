using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;

[ApiController]
[Route("api/[controller]")]
public class FuncoesController : ControllerBase
{
    private readonly SgsiDbContext _context;
    private readonly ILogger<FuncoesController> _logger;

    public FuncoesController(SgsiDbContext context, ILogger<FuncoesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Funcoes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Funcoes>>> GetFuncoes()
    {
        _logger.LogInformation("=== INICIANDO REQUISIÇÃO API FUNCOES ===");

        try
        {
            // Verifica se o DbContext está funcionando
            _logger.LogInformation("Verificando conexão com o banco...");

            var funcoes = await _context.Funcoes.AsNoTracking().OrderBy(f => f.ID).ToListAsync();

            _logger.LogInformation($"SUCESSO: {funcoes.Count} funções encontradas");

            if (funcoes.Count == 0)
            {
                _logger.LogWarning("Nenhuma função encontrada na tabela");
            }

            return Ok(funcoes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERRO CRÍTICO ao buscar funções");
            return StatusCode(
                500,
                new
                {
                    error = "Erro interno do servidor",
                    details = ex.Message,
                    stackTrace = ex.StackTrace,
                }
            );
        }
    }
}

