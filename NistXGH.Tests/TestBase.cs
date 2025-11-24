// NistXGH.Tests/TestBase.cs
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;

namespace NistXGH.Tests
{
    public abstract class TestBase : IDisposable
    {
        private bool _disposed = false;
        private readonly List<SgsiDbContext> _contexts = new();

        protected SgsiDbContext CreateMockDbContext()
        {
            var options = new DbContextOptionsBuilder<SgsiDbContext>()
                .UseInMemoryDatabase(databaseName: $"UnitTestDb_{Guid.NewGuid()}")
                .Options;

            var context = new SgsiDbContext(options);
            _contexts.Add(context);

            if (!context.Database.IsInMemory())
                throw new InvalidOperationException(
                    "🚨 Testes unitários devem usar APENAS banco em memória!"
                );

            SeedUnitTestData(context);
            return context;
        }

        protected void SeedUnitTestData(SgsiDbContext context)
        {
            try
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                // DADOS BÁSICOS PARA TESTES UNITÁRIOS
                var funcoes = new[]
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
                };
                context.Funcoes.AddRange(funcoes);

                var categorias = new[]
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
                    new Categorias
                    {
                        ID = 3,
                        CODIGO = "DE",
                        NOME = "Detecção",
                        FUNCAO = 2,
                    },
                };
                context.Categorias.AddRange(categorias);

                var subcategorias = new[]
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
                    new Subcategorias
                    {
                        ID = 3,
                        CATEGORIA = 2,
                        FUNCAO = 1,
                        SUBCATEGORIA = 1,
                        DESCRICAO = "Subcategoria PR-1",
                    },
                };
                context.Subcategorias.AddRange(subcategorias);

                var prioridades = new[]
                {
                    new PrioridadeTb { ID = 1, NIVEL = "ALTA" },
                    new PrioridadeTb { ID = 2, NIVEL = "MEDIA" },
                    new PrioridadeTb { ID = 3, NIVEL = "BAIXA" },
                };
                context.PrioridadeTb.AddRange(prioridades);

                var status = new[]
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
                    new StatusTb
                    {
                        ID = 3,
                        NIVEL = "NAO INICIADO",
                        STATUS = "Pendente",
                    },
                };
                context.StatusTb.AddRange(status);

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Erro no seed de testes unitários: {ex.Message}",
                    ex
                );
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var context in _contexts)
                    {
                        context?.Database?.EnsureDeleted();
                        context?.Dispose();
                    }
                    _contexts.Clear();
                }
                _disposed = true;
            }
        }

        //  MÉTODOS AUXILIARES 
        protected int GenerateTestId() => new Random().Next(100, 999);

        protected string GenerateTestString(string prefix = "Test") =>
            $"{prefix}_{Guid.NewGuid().ToString("N")[..8]}";
    }
}
