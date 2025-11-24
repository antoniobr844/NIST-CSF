using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NistXGH.Models;
using Xunit;

namespace NistXGH.Tests.Integration
{
    public class IntegrationTestBase : IAsyncLifetime
    {
        protected WebApplicationFactory<Program> Factory { get; private set; }

        protected HttpClient CreateClient() => Factory.CreateClient();

        private SgsiDbContext _context;
        private IServiceScope _scope;
        private static bool _databaseSeeded = false;
        private static readonly object _lock = new object();

        public IntegrationTestBase()
        {
            // Define a variável de ambiente para teste ANTES de criar a factory
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

            Factory = new WebApplicationFactory<Program>();
        }

        public async Task InitializeAsync()
        {
            _scope = Factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<SgsiDbContext>();

            // Garantir que o banco está criado
            await _context.Database.EnsureCreatedAsync();

            // Seed dos dados apenas uma vez
            lock (_lock)
            {
                if (!_databaseSeeded)
                {
                    SeedTestData(_context).Wait();
                    _databaseSeeded = true;
                }
            }
        }

        public async Task DisposeAsync()
        {
            // Não deletar o banco para evitar conflitos entre testes
            _scope?.Dispose();
            Factory?.Dispose();
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
        }

        private async Task SeedTestData(SgsiDbContext context)
        {
            // Verificar se já existem dados para evitar duplicação
            if (await context.Funcoes.AnyAsync())
                return;

            // Seed básico para testes de integração
            context.Funcoes.AddRange(
                new[]
                {
                    new Funcoes
                    {
                        ID = 1,
                        CODIGO = "GV",
                        NOME = "Governança",
                    },
                    new Funcoes
                    {
                        ID = 2,
                        CODIGO = "RS",
                        NOME = "Resiliência",
                    },
                }
            );

            context.Categorias.AddRange(
                new[]
                {
                    new Categorias
                    {
                        ID = 1,
                        CODIGO = "AC",
                        NOME = "Avaliação",
                        FUNCAO = 1,
                    },
                    new Categorias
                    {
                        ID = 2,
                        CODIGO = "PR",
                        NOME = "Proteção",
                        FUNCAO = 1,
                    },
                }
            );

            context.Subcategorias.AddRange(
                new[]
                {
                    new Subcategorias
                    {
                        ID = 1,
                        CATEGORIA = 1,
                        FUNCAO = 1,
                        SUBCATEGORIA = 1,
                        DESCRICAO = "Subcategoria AC-1",
                    },
                    new Subcategorias
                    {
                        ID = 2,
                        CATEGORIA = 1,
                        FUNCAO = 1,
                        SUBCATEGORIA = 2,
                        DESCRICAO = "Subcategoria AC-2",
                    },
                }
            );

            context.PrioridadeTb.AddRange(
                new[]
                {
                    new PrioridadeTb { ID = 1, NIVEL = "ALTA" },
                    new PrioridadeTb { ID = 2, NIVEL = "MEDIA" },
                }
            );

            context.StatusTb.AddRange(
                new[]
                {
                    new StatusTb
                    {
                        ID = 1,
                        NIVEL = "IMPLEMENTADO",
                        STATUS = "Concluído",
                    },
                    new StatusTb
                    {
                        ID = 2,
                        NIVEL = "EM ANDAMENTO",
                        STATUS = "Em Progresso",
                    },
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
