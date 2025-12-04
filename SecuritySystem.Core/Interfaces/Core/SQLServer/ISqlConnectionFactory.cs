using System.Data;

namespace SecuritySystem.Core.Interfaces.Core.SQLServer
{
    public interface ISqlConnectionFactory
    {
        IDbConnection Create();
    }
}
