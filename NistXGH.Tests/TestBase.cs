using Microsoft.EntityFrameworkCore;
using NistXGH.Models;

namespace NistXGH.Tests
{
    public abstract class TestBase : IDisposable
    {
        private bool _disposed = false;
        private List<SgsiDbContext> _contexts = new List<SgsiDbContext>();

        protected SgsiDbContext CreateMockDbContext()
        {
            var options = new DbContextOptionsBuilder<SgsiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SgsiDbContext(options);
            _contexts.Add(context);
            SeedTestData(context);
            return context;
        }

        protected void SeedTestData(SgsiDbContext context)
        {
            try
            {
                // Garantir que o banco está criado
                context.Database.EnsureCreated();

                // Limpar dados existentes
                context.CenariosAtual.RemoveRange(context.CenariosAtual);
                context.CenariosFuturo.RemoveRange(context.CenariosFuturo);
                context.Funcoes.RemoveRange(context.Funcoes);
                context.Categorias.RemoveRange(context.Categorias);
                context.Subcategorias.RemoveRange(context.Subcategorias);
                context.PrioridadeTb.RemoveRange(context.PrioridadeTb);
                context.StatusTb.RemoveRange(context.StatusTb);
                context.CenarioLog.RemoveRange(context.CenarioLog);

                context.SaveChanges();

                // Adicionar dados de teste
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
                Console.WriteLine($"Erro no seed: {ex.Message}");
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
                        context?.Dispose();
                    }
                    _contexts.Clear();
                }
                _disposed = true;
            }
        }
    }
}
