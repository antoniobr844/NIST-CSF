//using System.Diagnostics;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

//using NistXGH.Models;

namespace NistXGH.Controllers;

public class HomeController : Controller
{
    // Action para a página principal
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Consultas()
    {
        return View();
    }

    public IActionResult Precadastro()
    {
        return View();
    }

    public IActionResult Relatorios()
    {
        ViewData["Title"] = "Relatórios - Comparação de Cenários";
        return View();
    }

    public IActionResult Governanca()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    [HttpPost]
    public IActionResult SalvarAlteracoes(GovernancaViewModel model)
    {
        // Aqui você salva no banco de dados
        // model contém todas as alterações feitas

        return RedirectToAction("Sucesso");
    }

    public IActionResult Sucesso()
    {
        return View();
    }

    public IActionResult Identificar()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    [HttpPost]
    public IActionResult SalvarAlteracoes(IdentificarViewModel model)
    {
        // Aqui você salva no banco de dados
        // model contém todas as alterações feitas

        return RedirectToAction("Sucesso");
    }

    public IActionResult Proteger()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    [HttpPost]
    public IActionResult SalvarAlteracoes(ProtegerViewModel model)
    {
        // Aqui você salva no banco de dados
        // model contém todas as alterações feitas

        return RedirectToAction("Sucesso");
    }

    public IActionResult Detectar()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    [HttpPost]
    public IActionResult SalvarAlteracoes(DetectarViewModel model)
    {
        // Aqui você salva no banco de dados
        // model contém todas as alterações feitas

        return RedirectToAction("Sucesso");
    }

    public IActionResult Responder()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    [HttpPost]
    public IActionResult SalvarAlteracoes(ResponderViewModel model)
    {
        // Aqui você salva no banco de dados
        // model contém todas as alterações feitas

        return RedirectToAction("Sucesso");
    }

    public IActionResult Recuperar()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    [HttpPost]
    public IActionResult SalvarAlteracoes(RecuperarViewModel model)
    {
        // Aqui você salva no banco de dados
        // model contém todas as alterações feitas

        return RedirectToAction("Sucesso");
    }

    // API para obter as opções principais
    [HttpGet]
    public IActionResult GetOpcoesPrincipais()
    {
        var opcoes = new List<object>
        {
            new { Id = 1, Nome = "Governança" },
            new { Id = 2, Nome = "Identificar" },
            new { Id = 3, Nome = "Proteger" },
            new { Id = 4, Nome = "Detectar" },
            new { Id = 5, Nome = "Responder" },
            new { Id = 6, Nome = "Recuperar" },
        };

        return Json(opcoes);
    }

    // API para obter subopções baseadas na opção principal selecionada
    [HttpGet]
    public IActionResult GetSubOpcoes(int opcaoId)
    {
        var subOpcoes = new List<object>();

        switch (opcaoId)
        {
            case 1: // governança
                subOpcoes.AddRange(
                    new List<object>
                    {
                        new { Id = 1, Nome = "GV.OC" },
                        new { Id = 2, Nome = "GV.RM" },
                        new { Id = 3, Nome = "GV.RR" },
                        new { Id = 4, Nome = "GV.PO" },
                        new { Id = 5, Nome = "GV.OV" },
                        new { Id = 6, Nome = "GV.SC" },
                    }
                );
                break;
            case 2: // Identificar
                subOpcoes.AddRange(
                    new List<object>
                    {
                        new { Id = 201, Nome = "ID.AM" },
                        new { Id = 202, Nome = "ID.AC" },
                        new { Id = 203, Nome = "ID.IM" },
                    }
                );
                break;
            case 3: // Proteger
                subOpcoes.AddRange(
                    new List<object>
                    {
                        new { Id = 301, Nome = "PR.AA" },
                        new { Id = 302, Nome = "PR.AT" },
                        new { Id = 303, Nome = "PR.DS" },
                        new { Id = 304, Nome = "PR.PS" },
                        new { Id = 305, Nome = "PR.IR" },
                    }
                );
                break;
            case 4: // Detectar
                subOpcoes.AddRange(
                    new List<object>
                    {
                        new { Id = 401, Nome = "DE.AE" },
                        new { Id = 402, Nome = "DE.CM" },
                    }
                );
                break;
            case 5: // Responder
                subOpcoes.AddRange(
                    new List<object>
                    {
                        new { Id = 501, Nome = "RS.MA" },
                        new { Id = 502, Nome = "RS.CO" },
                        new { Id = 503, Nome = "RS.AN" },
                        new { Id = 504, Nome = "RS.MI" },
                    }
                );
                break;
            case 6: // Recuperar
                subOpcoes.AddRange(
                    new List<object>
                    {
                        new { Id = 601, Nome = "RC.RP" },
                        new { Id = 603, Nome = "RC.CO" },
                    }
                );
                break;
        }

        return Json(subOpcoes);
    }
}

/*public class ErrorViewModel
{
    public string RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}*/
