//using System.Diagnostics;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NistXGH.Models;

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
    public IActionResult PrecadastroAtual()
    {
        return View();
    }
    public IActionResult CenarioAF()
    {
        return View();
    }
    public IActionResult Relatorios()
    {
        ViewData["Title"] = "Relatórios - Comparação de Cenários";
        return View();
    }

    public IActionResult Gestao()
    {
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

    public IActionResult GovernancaAtual()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    public IActionResult IdentificarAtual()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    public IActionResult ProtegerAtual()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    public IActionResult DetectarAtual()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    public IActionResult ResponderAtual()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }

    public IActionResult RecuperarAtual()
    {
        // Recupera as opções selecionadas
        var opcoesJson = TempData["OpcoesSelecionadas"] as string;
        var opcoes = string.IsNullOrEmpty(opcoesJson)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(opcoesJson);

        ViewBag.OpcoesSelecionadas = opcoes;
        return View();
    }
}