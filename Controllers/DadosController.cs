// essa classe reflete o controller responsável por gerenciar as operações de cadastro e consulta de dados
using Microsoft.AspNetCore.Mvc;

namespace NistXGH.Controllers;

[ApiController]
[Route("api/[controller]")]
// esse arquivo é responsável por gerenciar as operações de cadastro e consulta de dados
public class DadosController(IDadosService dadosService) : ControllerBase
{
    private readonly IDadosService _dadosService = dadosService;

    [HttpPost("cadastrar")]
    public IActionResult Cadastrar([FromBody] Dados dados)
    {
        _dadosService.Cadastrar(dados);
        return Ok(new { success = true });
    }

    [HttpGet("consultar")]
    public IActionResult Consultar()
    {
        var dados = _dadosService.ConsultarTodos();
        return Ok(dados);
    }

    [HttpGet("consultar/{id}")]
    public IActionResult ConsultarPorId(int id)
    {
        var dado = _dadosService.ConsultarPorId(id);
        if (dado == null)
        {
            return NotFound();
        }
        return Ok(dado);
    }
}
