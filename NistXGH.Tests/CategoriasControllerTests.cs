using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using NistXGH.Controllers;
using NistXGH.Models;

namespace NistXGH.Tests.Unit
{
    public class CategoriasControllerTests
    {
        private SgsiDbContext CriarDbContext()
        {
            var options = new DbContextOptionsBuilder<SgsiDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new SgsiDbContext(options);

            context.Categorias.AddRange(
                new Categorias { ID = 1, NOME = "Teste 1", FUNCAO = 1 },
                new Categorias { ID = 2, NOME = "Teste 2", FUNCAO = 2 }
            );
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task DeveRetornarTodasAsCategorias()
        {
            var context = CriarDbContext();
            var controller = new CategoriasController(context);

            var result = await controller.GetCategorias(null);
            var ok = result.Result as OkObjectResult;

            Assert.NotNull(ok);

            var lista = ok.Value as List<Categorias>;
            Assert.Equal(2, lista.Count);
        }

        [Fact]
        public async Task DeveFiltrarPorFuncao()
        {
            var context = CriarDbContext();
            var controller = new CategoriasController(context);

            var result = await controller.GetCategorias(1);
            var ok = result.Result as OkObjectResult;

            Assert.NotNull(ok);
            var lista = ok.Value as List<Categorias>;

            Assert.Single(lista);
            Assert.Equal(1, lista.First().FUNCAO);
        }
    }
}
