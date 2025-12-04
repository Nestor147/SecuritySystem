using System.Data;

namespace SecuritySystem.Core.Interfaces.Core.SQLServer.ADO
{
    public interface IAdo
    {
        Task<List<RowEntity>> ExecuteEntitiesAsync(string nameOrSql, CommandType type, object? parameters = null, int? timeout = null, ListHandling listHandling = ListHandling.AsJson, string? tvpTypeNameForLists = null);
        Task<RowEntity?> ExecuteSingleEntityAsync(string nameOrSql, CommandType type, object? parameters = null, int? timeout = null, ListHandling listHandling = ListHandling.AsJson, string? tvpTypeNameForLists = null);
    }
}
