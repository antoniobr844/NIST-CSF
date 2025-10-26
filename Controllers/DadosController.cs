using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;

[ApiController]
[Route("api/[controller]")]
public class DadosController : ControllerBase
{
    private readonly SgsiDbContext _context;
    private readonly ILogger<DadosController> _logger;

    public DadosController(SgsiDbContext context, ILogger<DadosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/Catalogo/prioridades
    [HttpGet("prioridades")]
    public async Task<ActionResult<IEnumerable<PrioridadeTb>>> GetPrioridades()
    {
        _logger.LogInformation("=== INICIANDO REQUISIÇÃO API PRIORIDADES ===");

        try
        {
            _logger.LogInformation("Verificando conexão com o banco...");

            var prioridades = await _context
                .PrioridadeTb.AsNoTracking()
                .OrderBy(p => p.ID)
                .ToListAsync();

            _logger.LogInformation($"SUCESSO: {prioridades.Count} prioridades encontradas");

            if (prioridades.Count == 0)
            {
                _logger.LogWarning("Nenhuma prioridade encontrada na tabela SGSI_PRIORIDADE_TB");
            }

            return Ok(prioridades);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERRO CRÍTICO ao buscar prioridades");
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

    [HttpGet("prioridades/{id}")]
    public async Task<ActionResult<PrioridadeTb>> GetPrioridade(int id)
    {
        _logger.LogInformation($"=== BUSCANDO PRIORIDADE ID: {id} ===");

        try
        {
            var prioridade = await _context
                .PrioridadeTb.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ID == id);

            if (prioridade == null)
            {
                _logger.LogWarning($"Prioridade ID {id} não encontrada");
                return NotFound(new { message = "Prioridade não encontrada" });
            }

            _logger.LogInformation($"SUCESSO: Prioridade {prioridade.NIVEL} encontrada");
            return Ok(prioridade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ERRO ao buscar prioridade ID {id}");
            return StatusCode(
                500,
                new { error = "Erro interno do servidor", details = ex.Message }
            );
        }
    }

    [HttpGet("status")]
    public async Task<ActionResult<IEnumerable<StatusTb>>> GetStatus()
    {
        _logger.LogInformation("=== INICIANDO REQUISIÇÃO API STATUS ===");

        try
        {
            _logger.LogInformation("Verificando conexão com o banco...");

            var status = await _context.StatusTb.AsNoTracking().OrderBy(s => s.ID).ToListAsync();

            _logger.LogInformation($"SUCESSO: {status.Count} status encontrados");

            if (status.Count == 0)
            {
                _logger.LogWarning("Nenhum status encontrado na tabela SGSI_STATUS_TB");
            }

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERRO CRÍTICO ao buscar status");
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

    // GET: api/Catalogo/status/5
    [HttpGet("status/{id}")]
    public async Task<ActionResult<StatusTb>> GetStatus(int id)
    {
        _logger.LogInformation($"=== BUSCANDO STATUS ID: {id} ===");

        try
        {
            var status = await _context
                .StatusTb.AsNoTracking()
                .FirstOrDefaultAsync(s => s.ID == id);

            if (status == null)
            {
                _logger.LogWarning($"Status ID {id} não encontrado");
                return NotFound(new { message = "Status não encontrado" });
            }

            _logger.LogInformation($"SUCESSO: Status {status.NIVEL} encontrado");
            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"ERRO ao buscar status ID {id}");
            return StatusCode(
                500,
                new { error = "Erro interno do servidor", details = ex.Message }
            );
        }
    }

    // GET: api/Catalogo/todos - Retorna prioridades e status em uma única chamada
    [HttpGet("todos")]
    public async Task<ActionResult<object>> GetTodosCatalogos()
    {
        _logger.LogInformation("=== INICIANDO REQUISIÇÃO TODOS OS CATALOGOS ===");

        try
        {
            var prioridades = await _context
                .PrioridadeTb.AsNoTracking()
                .OrderBy(p => p.ID)
                .ToListAsync();

            var status = await _context.StatusTb.AsNoTracking().OrderBy(s => s.ID).ToListAsync();

            _logger.LogInformation(
                $"SUCESSO: {prioridades.Count} prioridades e {status.Count} status encontrados"
            );

            return Ok(new { prioridades, status });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERRO CRÍTICO ao buscar catalogos");
            return StatusCode(
                500,
                new { error = "Erro interno do servidor", details = ex.Message }
            );
        }
    }
}
