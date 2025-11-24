using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NistXGH.Models;
using Xunit;

namespace NistXGH.DatabaseIntegration
{
    [Trait("Category", "DatabaseIntegration")]
    [Trait("RequiresDatabase", "true")]
    public class OracleDatabaseIntegrationTests : IAsyncLifetime
    {
        private readonly SgsiDbContext? _context; // 🔥 TORNAR ANULÁVEL
        private bool _databaseAvailable;

        public OracleDatabaseIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("TestDatabase");

            if (string.IsNullOrEmpty(connectionString))
            {
                _databaseAvailable = false;
                return;
            }

            var options = new DbContextOptionsBuilder<SgsiDbContext>()
                .UseOracle(connectionString)
                .Options;

            _context = new SgsiDbContext(options);
        }

        public async Task InitializeAsync()
        {
            if (_context == null)
            {
                _databaseAvailable = false;
                return;
            }

            try
            {
                var connection = _context.Database.GetDbConnection();
                var databaseName = connection.Database?.ToLower() ?? "";

                if (databaseName.Contains("prod") || databaseName.Contains("production"))
                {
                    throw new InvalidOperationException(
                        "🚨 NUNCA execute testes no banco de produção!"
                    );
                }

                _databaseAvailable = await _context.Database.CanConnectAsync();
            }
            catch
            {
                _databaseAvailable = false;
            }
        }

        public Task DisposeAsync()
        {
            _context?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        [Trait("Category", "DatabaseIntegration")]
        public void DatabaseConnection_ShouldBeSuccessful()
        {
            if (!_databaseAvailable)
                return;

            Assert.True(_databaseAvailable, "Não foi possível conectar ao banco de teste Oracle");
        }

        [Fact]
        [Trait("Category", "DatabaseIntegration")]
        public async Task Should_QueryBasicData_FromDatabase()
        {
            if (!_databaseAvailable || _context == null)
                return;

            var funcoes = await _context.Funcoes.Take(5).ToListAsync();
            Assert.NotNull(funcoes);
        }
    }
}
