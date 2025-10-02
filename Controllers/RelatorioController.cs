// Controllers/RelatorioController.cs
using Microsoft.AspNetCore.Mvc;

public class RelatorioController : Controller
{
    private readonly IRelatorioService _relatorioService;

    public RelatorioController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    // Controller correto
    public IActionResult Relatorios(bool? mostrarFuturo)
    {
        var viewModel = new RelatorioViewModel
        {
            Dados = _relatorioService.ObterTodosDados(),
            MostrarCenarioFuturo = mostrarFuturo ?? false,
        };

        // Verifique se os dados não são nulos
        if (viewModel.Dados == null)
        {
            viewModel.Dados = new List<Relatorio>(); // Lista vazia instead of null
        }

        viewModel.TotalAtual = viewModel.Dados.Sum(d => d.ValorAtual);
        viewModel.TotalFuturo = viewModel.Dados.Sum(d => d.ValorFuturo);

        return View(viewModel); // ← Aqui deve passar o modelo
    }

    [HttpPost]
    public IActionResult ToggleCenario(bool mostrarFuturo)
    {
        return RedirectToAction("Relatorios", new { mostrarFuturo });
    }
}

// Services/IRelatorioService.cs
public interface IRelatorioService
{
    List<Relatorio> ObterTodosDados();
}

// Services/RelatorioService.cs
public class RelatorioService : IRelatorioService
{
    // Simulação de dados - na prática viria do banco de dados
    public List<Relatorio> ObterTodosDados()
    {
        return new List<Relatorio>
        {
            new()
            {
                Id = 1,
                Nome = "Projeto Alpha",
                Categoria = "Desenvolvimento",
                ValorAtual = 15000,
                ValorFuturo = 18000,
                DataCriacao = DateTime.Now.AddDays(-30),
                Status = "Ativo",
            },
            new()
            {
                Id = 2,
                Nome = "Projeto Beta",
                Categoria = "Marketing",
                ValorAtual = 8000,
                ValorFuturo = 12000,
                DataCriacao = DateTime.Now.AddDays(-15),
                Status = "Pendente",
            },
            new()
            {
                Id = 3,
                Nome = "Projeto Gamma",
                Categoria = "Infraestrutura",
                ValorAtual = 25000,
                ValorFuturo = 22000,
                DataCriacao = DateTime.Now.AddDays(-7),
                Status = "Concluído",
            },
            new()
            {
                Id = 4,
                Nome = "Projeto Delta",
                Categoria = "Pesquisa",
                ValorAtual = 12000,
                ValorFuturo = 15000,
                DataCriacao = DateTime.Now.AddDays(-3),
                Status = "Ativo",
            },
        };
    }
}
