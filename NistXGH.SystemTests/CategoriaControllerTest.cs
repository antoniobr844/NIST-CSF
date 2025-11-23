using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;
using System.Linq;
using System.Net.Http.Json;


namespace NISTCSF.SystemTests
{
    public class CategoriasControllerTests :
        IClassFixture<WebApplicationFactory<NISTCSF.Program>>
    {
        private readonly WebApplicationFactory<NISTCSF.Program> _factory;

        public CategoriasControllerTests(
            WebApplicationFactory<NISTCSF.Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove o DbContext real (Oracle)
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<SgsiDbContext>)
                    );

                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Registramos o banco InMemory para testes
                    services.AddDbContext<SgsiDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });

                    // Cria dados iniciais
                    using var scope = services.BuildServiceProvider().CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SgsiDbContext>();

                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();


                    db.Categorias.AddRange(
                        new Categorias { ID = 1, NOME = "Categoria A", FUNCAO = 1 },
                        new Categorias { ID = 2, NOME = "Categoria B", FUNCAO = 2 },
                        new Categorias { ID = 3, NOME = "Categoria C", FUNCAO = 1 }
                    );

                    db.SaveChanges();
                });
            });
        }

        [Fact]
        public async Task DeveRetornarTodasCategorias()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Categorias");

            response.EnsureSuccessStatusCode();

            var categorias = await response.Content.ReadFromJsonAsync<List<Categorias>>();

            Assert.NotNull(categorias);
            Assert.Equal(3, categorias.Count);
        }

        [Fact]
        public async Task DeveFiltrarPorFuncao()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Categorias?funcaoId=1");

            response.EnsureSuccessStatusCode();

            var categorias = await response.Content.ReadFromJsonAsync<List<Categorias>>();

            Assert.NotNull(categorias);

            // Apenas categorias com FUNCAO = 1
            Assert.All(categorias, c => Assert.Equal(1, c.FUNCAO));

            // Verifica nomes esperados
            Assert.Contains(categorias, c => c.NOME == "Categoria A");
            Assert.Contains(categorias, c => c.NOME == "Categoria C");
        }
    }
}
