using Microsoft.Data.SqlClient;
using SecuritySystem.Core.Interfaces.Core.SQLServer;
using SecuritySystem.Core.Interfaces.Core.SQLServer.ADO;
using System.Data;

namespace SecuritySystem.Infrastructure.Context.Core.SQLServer.ADO
{
    public class Ado : IAdo
    {
        private readonly ISqlConnectionFactory _factory;
        public Ado(ISqlConnectionFactory factory) => _factory = factory;

        public async Task<List<RowEntity>> ExecuteEntitiesAsync(
            string nameOrSql,
            CommandType type,
            object? parameters = null,
            int? timeout = null,
            ListHandling listHandling = ListHandling.AsJson,
            string? tvpTypeNameForLists = null
        )
        {
            using var conn = (SqlConnection)_factory.Create();
            using var cmd = new SqlCommand(nameOrSql, conn) { CommandType = type };
            if (timeout.HasValue) cmd.CommandTimeout = timeout.Value;
            var sqlParams = SqlParamsADO.ToSqlParameters(parameters, listHandling, tvpTypeNameForLists);

            if (sqlParams.Length > 0) cmd.Parameters.AddRange(sqlParams);

            if (conn.State == ConnectionState.Closed) await conn.OpenAsync();

            var list = new List<RowEntity>();
            using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            while (await reader.ReadAsync())
            {
                var dict = new Dictionary<string, object?>(reader.FieldCount, StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < reader.FieldCount; i++)
                    dict[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);

                list.Add(new RowEntity(dict));
            }
            return list;
        }
        public async Task<RowEntity?> ExecuteSingleEntityAsync(
            string nameOrSql,
            CommandType type,
            object? parameters = null,
            int? timeout = null,
            ListHandling listHandling = ListHandling.AsJson,
            string? tvpTypeNameForLists = null
        )
        {
            var rows = await ExecuteEntitiesAsync(nameOrSql, type, parameters, timeout, listHandling, tvpTypeNameForLists);
            return rows.Count > 0 ? rows[0] : null;
        }


    }
}
