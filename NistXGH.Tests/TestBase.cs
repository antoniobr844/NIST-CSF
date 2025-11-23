using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NistXGH.Models;

namespace NistXGH.Tests
{
    public abstract class TestBase : IDisposable
    {
        protected SgsiDbContext CreateMockDbContext()
        {
            var options = new DbContextOptionsBuilder<SgsiDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            return new SgsiDbContext(options);
        }

        protected void SeedTestData(SgsiDbContext context)
        {
            // Adicionar dados básicos para teste - CORRIGIDO com base nos Models reais
            if (!context.Funcoes.Any())
            {
                context.Funcoes.AddRange(
                    new Funcoes
                    {
                        ID = 1,
                        CODIGO = "GV",
                        NOME = "Governança",
                        DESCRICAO = "Função de Governança",
                    },
                    new Funcoes
                    {
                        ID = 2,
                        CODIGO = "ID",
                        NOME = "Identificar",
                        DESCRICAO = "Função de Identificação",
                    }
                );
            }

            if (!context.Categorias.Any())
            {
                context.Categorias.AddRange(
                    new Categorias
                    {
                        ID = 1,
                        FUNCAO = 1,
                        CODIGO = "GV.AC",
                        NOME = "Controle de Acesso",
                        DESCRICAO = "Categoria de Controle de Acesso",
                    },
                    new Categorias
                    {
                        ID = 2,
                        FUNCAO = 1,
                        CODIGO = "GV.AT",
                        NOME = "Ameaças",
                        DESCRICAO = "Categoria de Ameaças",
                    }
                );
            }

            if (!context.Subcategorias.Any())
            {
                context.Subcategorias.AddRange(
                    // CORREÇÃO: Baseado no Model real - SUBCATEGORIA é int?
                    new Subcategorias
                    {
                        ID = 1,
                        CATEGORIA = 1,
                        FUNCAO = 1,
                        SUBCATEGORIA = 1,
                        DESCRICAO = "Identificar usuários",
                    },
                    new Subcategorias
                    {
                        ID = 2,
                        CATEGORIA = 1,
                        FUNCAO = 1,
                        SUBCATEGORIA = 2,
                        DESCRICAO = "Autenticar usuários",
                    }
                );
            }

            if (!context.PrioridadeTb.Any())
            {
                context.PrioridadeTb.AddRange(
                    new PrioridadeTb { ID = 1, NIVEL = "1" },
                    new PrioridadeTb { ID = 2, NIVEL = "2" },
                    new PrioridadeTb { ID = 3, NIVEL = "3" }
                );
            }

            if (!context.StatusTb.Any())
            {
                context.StatusTb.AddRange(
                    new StatusTb
                    {
                        ID = 1,
                        NIVEL = "Implementado",
                        STATUS = "Ativo",
                    },
                    new StatusTb
                    {
                        ID = 2,
                        NIVEL = "Parcial",
                        STATUS = "Ativo",
                    },
                    new StatusTb
                    {
                        ID = 3,
                        NIVEL = "Não Implementado",
                        STATUS = "Ativo",
                    }
                );
            }

            context.SaveChanges();
        }

        public void Dispose()
        {
            // Limpar recursos se necessário
        }
    }
}
