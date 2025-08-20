using System.Data;

using RS.Dapper.Utility.Constants;
using RS.Dapper.Utility.Models;

namespace RS.Dapper.Utility.Resolver;
public interface IDatabaseResolver
{
   public IDbConnection GetConnection(string databaseName);
   public DatabaseType GetDatabaseType(string dbName);
   public DatabaseConfig GetConfig(string databaseName);
}
