using Microsoft.AspNetCore.Mvc;
using NistXGH.Models;

public class ResponderController : Controller
{
    // Action para a página de alterações
    public IActionResult GetPrecadastro()
    {
        return View();
    }
}
