// NistXGH.Tests/Integration/IntegrationTestBase.cs
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NistXGH.Models;
using Xunit;

namespace NistXGH.Tests.Integration
{
    public class IntegrationTestBase : IAsyncLifetime, IDisposable
    {
        protected CustomWebApplicationFactory Factory { get; private set; }

        protected HttpClient CreateClient() => Factory.CreateClient();

        private SgsiDbContext? _context;
        private IServiceScope? _scope;
        protected readonly string _databaseName; 
        private bool _disposed = false;

        public IntegrationTestBase()
        {
            _databaseName = $"TestDb_{Guid.NewGuid()}";
            Factory = new CustomWebApplicationFactory(_databaseName);
        }

        public async Task InitializeAsync()
        {
            _scope = Factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<SgsiDbContext>();

            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();

            await SeedEssentialTestData(_context);
        }

        public async Task DisposeAsync()
        {
            if (_disposed)
                return;

            if (_context != null)
            {
                await _context.Database.EnsureDeletedAsync();
                _context.Dispose();
            }
            _scope?.Dispose();
            Factory?.Dispose();

            _disposed = true;
        }

        public void Dispose() => DisposeAsync().GetAwaiter().GetResult();

        private async Task SeedEssentialTestData(SgsiDbContext context)
        {
           
            if (context.CenariosAtual.Any())
                context.CenariosAtual.RemoveRange(context.CenariosAtual);
            if (context.CenariosFuturo.Any())
                context.CenariosFuturo.RemoveRange(context.CenariosFuturo);
            if (context.Funcoes.Any())
                context.Funcoes.RemoveRange(context.Funcoes);
            if (context.Categorias.Any())
                context.Categorias.RemoveRange(context.Categorias);
            if (context.Subcategorias.Any())
                context.Subcategorias.RemoveRange(context.Subcategorias);
            if (context.PrioridadeTb.Any())
                context.PrioridadeTb.RemoveRange(context.PrioridadeTb);
            if (context.StatusTb.Any())
                context.StatusTb.RemoveRange(context.StatusTb);

            await context.SaveChangesAsync();
            if (!await context.Funcoes.AnyAsync())
            {
                context.Funcoes.AddRange(
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
                    }
                );
            }

            if (!await context.Categorias.AnyAsync())
            {
                context.Categorias.AddRange(
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
                    }
                );
            }

            if (!await context.Subcategorias.AnyAsync())
            {
                context.Subcategorias.AddRange(
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
                    }
                );
            }

            if (!await context.PrioridadeTb.AnyAsync())
            {
                context.PrioridadeTb.AddRange(
                    new PrioridadeTb { ID = 1, NIVEL = "ALTA" },
                    new PrioridadeTb { ID = 2, NIVEL = "MEDIA" }
                );
            }

            if (!await context.StatusTb.AnyAsync())
            {
                context.StatusTb.AddRange(
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
                    }
                );
            }

            await context.SaveChangesAsync();
        }

        // Métodos auxiliares
        protected int GenerateUniqueId() => new Random().Next(1000, 9999);

        protected string GenerateUniqueString(string prefix = "") =>
            $"{prefix}_{Guid.NewGuid().ToString("N")[..8]}";
    }
}
