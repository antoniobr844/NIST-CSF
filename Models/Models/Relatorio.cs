// Models/Relatorio.cs
public class Relatorio
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Categoria { get; set; }
    public decimal ValorAtual { get; set; }
    public decimal ValorFuturo { get; set; }
    public DateTime DataCriacao { get; set; }
    public string? Status { get; set; }
}

// Models/RelatorioViewModel.cs
public class RelatorioViewModel
{
    public List<Relatorio>? Dados { get; set; }
    public bool MostrarCenarioFuturo { get; set; }
    public decimal TotalAtual { get; set; }
    public decimal TotalFuturo { get; set; }
}
