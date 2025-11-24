// NistXGH.Tests/Integration/CustomWebApplicationFactory.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NistXGH.Models;

namespace NistXGH.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName;

        public CustomWebApplicationFactory(string databaseName)
        {
            _databaseName = databaseName ?? $"TestDb_{Guid.NewGuid()}";
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove configuração real do DbContext
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<SgsiDbContext>)
                );

                if (descriptor != null)
                    services.Remove(descriptor);

                // Adiciona banco em memória
                services.AddDbContext<SgsiDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                    options.EnableSensitiveDataLogging();
                });

                // Validação de segurança
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<SgsiDbContext>();

                    try
                    {
                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(
                            "Não foi possível configurar o banco de testes. Verifique se está usando InMemory.",
                            ex
                        );
                    }
                }
            });

            builder.UseEnvironment("Development");
        }

        public void ValidateDatabaseSafety()
        {
            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SgsiDbContext>();

            if (!dbContext.Database.IsInMemory())
                throw new InvalidOperationException("🚨 PERIGO: Banco de dados NÃO é em memória!");
        }
    }
}
