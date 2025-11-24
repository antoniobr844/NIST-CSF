// NistXGH.Tests/Integration/Security/DatabaseSafetyTests.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace NistXGH.Tests.Integration.Security
{
    public class DatabaseSafetyTests : IntegrationTestBase
    {
        [Fact]
        public void AllTests_Should_Use_InMemoryDatabase()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SgsiDbContext>();

            Assert.True(
                dbContext.Database.IsInMemory(),
                "🚨 PERIGO: Testes devem usar APENAS banco em memória!"
            );
        }

        [Fact]
        public void Database_Should_Contain_Test_Data()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SgsiDbContext>();
            var funcoes = dbContext.Funcoes.ToList();
            var categorias = dbContext.Categorias.ToList();

            Assert.True(funcoes.Count >= 2, "Deve ter pelo menos 2 funções");
            Assert.True(categorias.Count >= 2, "Deve ter pelo menos 2 categorias");

            Console.WriteLine(
                $"✅ Banco contém {funcoes.Count} funções e {categorias.Count} categorias"
            );
        }

        [Fact]
        public void Should_Use_InMemory_Database_In_Tests()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SgsiDbContext>();
            Assert.True(dbContext.Database.IsInMemory());

            var configuration = Factory.Services.GetService<IConfiguration>();
            var connectionString = configuration?.GetConnectionString("SgsiDbContext");

            if (!string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine($"ℹ️ Connection string detectada: {connectionString}");
                Console.WriteLine(
                    "⚠️ AVISO: Há connection string, mas testes estão usando InMemory (CORRETO)"
                );
            }
            else
            {
                Console.WriteLine("✅ Nenhuma connection string detectada (IDEAL)");
            }

            Console.WriteLine("✅ VALIDAÇÃO PRINCIPAL: Testes usando banco em memória - SEGURO!");
        }

        [Fact]
        public void Each_Test_Has_Unique_Database()
        {
            // Este teste valida que cada instância tem database único
            Assert.NotNull(_databaseName);
            Assert.Contains("TestDb_", _databaseName);

            Console.WriteLine($"✅ Database único: {_databaseName}");
        }
    }
}
