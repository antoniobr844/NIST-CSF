using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NistXGH.Models;

namespace NistXGH.Controllers
{
    public class CenarioAtualController : Controller
    {
        // Simulação de banco de dados em memória
        private static List<CenarioAtual> _CenarioAtual = new List<CenarioAtual>();
        private static int _nextId = 1;

        // GET: Categoria
        public IActionResult Index()
        {
            return View(_CenarioAtual.OrderBy(c => c.JUSTIFICATIVA).ToList());
        }

        // GET: Categoria/Cadastrar
        public IActionResult Cadastrar()
        {
            return View();
        }

        // POST: Categoria/Cadastrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cadastrar(CenarioAtual cenarioAtual)
        {
            if (ModelState.IsValid)
            {
                cenarioAtual.ID = _nextId++;
                _CenarioAtual.Add(cenarioAtual);
                return RedirectToAction(nameof(Index));
            }
            return View(cenarioAtual);
        }

        // GET: Categoria/Editar/5
        public IActionResult Editar(int id)
        {
            var cenarioAtual = _CenarioAtual.FirstOrDefault(c => c.ID == id);
            if (cenarioAtual == null)
            {
                return NotFound();
            }
            return View(cenarioAtual);
        }

        // POST: Categoria/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, CenarioAtual cenarioAtual)
        {
            if (id != cenarioAtual.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingCenarioAtual = _CenarioAtual.FirstOrDefault(c => c.ID == id);
                if (existingCenarioAtual == null)
                {
                    return NotFound();
                }

                existingCenarioAtual.PRIOR_ATUAL = cenarioAtual.PRIOR_ATUAL;
                existingCenarioAtual.NIVEL_ATUAL = cenarioAtual.NIVEL_ATUAL;
                existingCenarioAtual.POLIT_ATUAL = cenarioAtual.POLIT_ATUAL;
                existingCenarioAtual.PRAT_ATUAL = cenarioAtual.PRAT_ATUAL;
                existingCenarioAtual.FUNC_RESP = cenarioAtual.FUNC_RESP;
                existingCenarioAtual.REF_INFO = cenarioAtual.REF_INFO;
                existingCenarioAtual.EVID_ATUAL = cenarioAtual.EVID_ATUAL;
                existingCenarioAtual.SUBCATEGORIA = cenarioAtual.SUBCATEGORIA;
                existingCenarioAtual.DATA_REGISTRO = cenarioAtual.DATA_REGISTRO;

                return RedirectToAction(nameof(Index));
            }
            return View(cenarioAtual);
        }

        // GET: Categoria/Excluir/5
        public IActionResult Excluir(int id)
        {
            var cenarioAtual = _CenarioAtual.FirstOrDefault(c => c.ID == id);
            if (cenarioAtual == null)
            {
                return NotFound();
            }
            return View(cenarioAtual);
        }

        // POST: Categoria/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarExcluir(int id)
        {
            var cenarioAtual = _CenarioAtual.FirstOrDefault(c => c.ID == id);
            if (cenarioAtual != null)
            {
                _CenarioAtual.Remove(cenarioAtual);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
