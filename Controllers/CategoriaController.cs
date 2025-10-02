// Controllers/CategoriaController.cs
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NistXGH.Models;

namespace NistXGH.Controllers
{
    public class CategoriaController : Controller
    {
        // Simulação de banco de dados em memória
        private static List<Categoria> _categorias = new List<Categoria>();
        private static int _nextId = 1;

        // GET: Categoria
        public IActionResult Index()
        {
            return View(_categorias.OrderBy(c => c.Justificativa).ToList());
        }

        // GET: Categoria/Cadastrar
        public IActionResult Cadastrar()
        {
            return View();
        }

        // POST: Categoria/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                categoria.Id = _nextId++;
                _categorias.Add(categoria);
                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categoria/Editar/5
        public IActionResult Editar(int id)
        {
            var categoria = _categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categoria/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingCategoria = _categorias.FirstOrDefault(c => c.Id == id);
                if (existingCategoria == null)
                {
                    return NotFound();
                }

                existingCategoria.Resultado = categoria.Resultado;
                existingCategoria.Descricao = categoria.Descricao;
                existingCategoria.Justificativa = categoria.Justificativa;
                existingCategoria.IncluidoPerfil = categoria.IncluidoPerfil;
                existingCategoria.Prioridade = categoria.Prioridade;
                existingCategoria.Status = categoria.Status;
                existingCategoria.PoliticasPro = categoria.PoliticasPro;
                existingCategoria.PraticasInternas = categoria.PraticasInternas;
                existingCategoria.FuncoesResp = categoria.FuncoesResp;
                existingCategoria.ReferenciasInfo = categoria.ReferenciasInfo;
                existingCategoria.ArtefatosEvi = categoria.ArtefatosEvi;
                existingCategoria.Notas = categoria.Notas;
                existingCategoria.Consideracoes = categoria.Consideracoes;
                existingCategoria.Icone = categoria.Icone;

                return RedirectToAction(nameof(Index));
            }
            return View(categoria);
        }

        // GET: Categoria/Excluir/5
        public IActionResult Excluir(int id)
        {
            var categoria = _categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
            {
                return NotFound();
            }
            return View(categoria);
        }

        // POST: Categoria/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarExcluir(int id)
        {
            var categoria = _categorias.FirstOrDefault(c => c.Id == id);
            if (categoria != null)
            {
                _categorias.Remove(categoria);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
