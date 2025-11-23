using Xunit;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;


namespace NistXGH.Tests.Integration
{
    public class OracleConnectionTests
    {
        private readonly string _connectionString;

        public OracleConnectionTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            _connectionString = config.GetConnectionString("SgsiDbContext");
        }

        [Fact]
        public void DeveConectarAoBancoOracleComSucesso()
        {
            using var connection = new OracleConnection(_connectionString);

            connection.Open();

            Assert.Equal(System.Data.ConnectionState.Open, connection.State);

            connection.Close();
        }
    }
}
