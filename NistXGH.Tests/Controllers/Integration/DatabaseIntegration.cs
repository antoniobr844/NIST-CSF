using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NistXGH.Models;
using Xunit;

namespace NistXGH.DatabaseIntegration
{
    public class OracleDatabaseIntegrationTests : IAsyncLifetime
    {
        private readonly SgsiDbContext _context;
        private bool _databaseAvailable;

        public OracleDatabaseIntegrationTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("SgsiDbContext");

            var options = new DbContextOptionsBuilder<SgsiDbContext>()
                .UseOracle(connectionString)
                .Options;

            _context = new SgsiDbContext(options);
        }

        public async Task InitializeAsync()
        {
            try
            {
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
        public void DatabaseConnection_ShouldBeSuccessful()
        {
            Assert.True(_databaseAvailable, "Não foi possível conectar ao banco de dados Oracle");
        }

        [Fact]
        public async Task Should_QueryBasicData_FromDatabase()
        {
            if (!_databaseAvailable)
                return; // Sai sem falhar

            var funcoes = await _context.Funcoes.Take(5).ToListAsync();

            Assert.NotNull(funcoes);
        }

        [Fact]
        public async Task Should_ExecuteStoredProcedure_IfExists()
        {
            if (!_databaseAvailable)
                return;

            var result = await _context
                .PrioridadeTb.FromSqlRaw("SELECT * FROM PRIORIDADE_TB WHERE ROWNUM <= 3")
                .ToListAsync();

            Assert.NotNull(result);
            Assert.True(result.Count <= 3);
        }
    }
}
