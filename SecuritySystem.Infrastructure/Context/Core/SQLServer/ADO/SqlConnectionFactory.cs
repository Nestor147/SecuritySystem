using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SecuritySystem.Core.Interfaces.Core.SQLServer;
using System.Data;

namespace SecuritySystem.Infrastructure.Context.Core.SQLServer.ADO
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _cs;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _cs = configuration.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("ConnectionStrings:SqlServer no configurado");
        }
        public IDbConnection Create() => new SqlConnection(_cs);
    }
}
