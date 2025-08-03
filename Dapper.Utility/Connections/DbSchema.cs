using Microsoft.Extensions.Configuration;

using RS.Dapper.Utility.Constants;

namespace RS.Dapper.Utility.Connections;
public static class DbSchema
{
        private const string Schema = "dbo";
     //private const string Schema = "";// In MYSQL database case we canot use any Schema
     //public const string Schema = "public";// In PostgreSql database case we canot use any Schema
    // Tables
    public static string UserTable => GetTable($"User", Schema);
    public static string CategoryTable => GetTable($"Category",Schema);

    // Stored Procedures
    public static string UserGetByIdProc => GetTable($"User_GetById", Schema);
    public static string UserInsertProc => GetTable($"User_Insert", Schema);
    public static string UserUpdateProc => GetTable($"User_Update", Schema);

    private static DatabaseType _dbType = DatabaseType.SqlServer;
    public static void Initialize(IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("DatabaseSettings");
        Enum.TryParse(dbSettings["Type"], out _dbType);
    }

    public static string GetTable(string tableName, string schema="")
    {
        if (string.IsNullOrEmpty(schema))
        {
            schema = Schema;
        }
        return _dbType switch
        {
            DatabaseType.PostgreSql => $"\"{schema}\".\"{tableName}\"",
            DatabaseType.SqlServer => $"[{schema}].[{tableName}]",
            DatabaseType.MySql => $"`{tableName}`", // MySQL often doesn't use schema names
            _ => tableName
        };
    }

    public static string GetProcedure(string procName, string schema = "")
    {
        if (string.IsNullOrEmpty(schema))
        {
            schema = Schema;
        }
        return _dbType switch
        {
            DatabaseType.PostgreSql => $"\"{schema}\".\"{procName}\"",
            DatabaseType.SqlServer => $"[{schema}].[{procName}]",
            DatabaseType.MySql => $"`{procName}`",
            _ => procName
        };
    }
}
