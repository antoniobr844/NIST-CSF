// NistXGH.Tests/TestBase.cs
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;

namespace NistXGH.Tests
{
    public class TestBase
    {
        protected SgsiDbContext CreateMockDbContext()
        {
            var options = new DbContextOptionsBuilder<SgsiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new SgsiDbContext(options);
        }

        protected void SeedTestData(SgsiDbContext context)
        {
            // Limpar dados existentes primeiro para evitar conflitos
            context.Funcoes.RemoveRange(context.Funcoes);
            context.Categorias.RemoveRange(context.Categorias);
            context.Subcategorias.RemoveRange(context.Subcategorias);
            context.PrioridadeTb.RemoveRange(context.PrioridadeTb);
            context.StatusTb.RemoveRange(context.StatusTb);
            context.SaveChanges();

            // Adicionar Funções
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
                    CODIGO = "ID",
                    NOME = "Identificar",
                }
            );

            // Adicionar Categorias
            context.Categorias.AddRange(
                new Categorias
                {
                    ID = 1,
                    CODIGO = "AC",
                    NOME = "Análise de Contexto",
                    FUNCAO = 1,
                },
                new Categorias
                {
                    ID = 2,
                    CODIGO = "RM",
                    NOME = "Gerenciamento de Riscos",
                    FUNCAO = 1,
                }
            );

            // Adicionar Subcategorias - CORREÇÃO: SUBCATEGORIA é int, não string
            context.Subcategorias.AddRange(
                new Subcategorias
                {
                    ID = 1,
                    SUBCATEGORIA = 1, // 🔥 CORREÇÃO: número inteiro, não string
                    DESCRICAO = "Subcategoria 1",
                    CATEGORIA = 1,
                    FUNCAO = 1,
                },
                new Subcategorias
                {
                    ID = 2,
                    SUBCATEGORIA = 2, // 🔥 CORREÇÃO: número inteiro, não string
                    DESCRICAO = "Subcategoria 2",
                    CATEGORIA = 1,
                    FUNCAO = 1,
                }
            );

            // Adicionar Prioridades
            context.PrioridadeTb.AddRange(
                new PrioridadeTb { ID = 1, NIVEL = "ALTA" },
                new PrioridadeTb { ID = 2, NIVEL = "MEDIA" },
                new PrioridadeTb { ID = 3, NIVEL = "BAIXA" }
            );

            // Adicionar Status
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
                    NIVEL = "EM_ANDAMENTO",
                    STATUS = "Em Progresso",
                },
                new StatusTb
                {
                    ID = 3,
                    NIVEL = "NAO_INICIADO",
                    STATUS = "Não Iniciado",
                }
            );

            context.SaveChanges();
        }
    }
}
