using System.Data;

using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using RS.Dapper.Utility.Constants;
using RS.Dapper.Utility.Models;
namespace RS.Dapper.Utility.Resolver;
public class DatabaseResolver : IDatabaseResolver
{
    private readonly Dictionary<string, DatabaseConfig> _configs;

    public DatabaseResolver(IConfiguration configuration)
    {
        _configs = configuration.GetSection("Databases")
                                .GetChildren()
                                .ToDictionary(
                                    x => x.Key,
                                    x => new DatabaseConfig
                                    {
                                        DbType = Enum.Parse<DatabaseType>(x["DbType"] ?? "SqlServer"),
                                        ConnectionString = x["ConnectionString"]
                                    });
    }

    public IDbConnection GetConnection(string databaseName)
    {
        var config = GetConfig(databaseName);
        return config.DbType switch
        {
            DatabaseType.SqlServer => new SqlConnection(config.ConnectionString),
            DatabaseType.MySql => new MySql.Data.MySqlClient.MySqlConnection(config.ConnectionString),
            DatabaseType.PostgreSql => new Npgsql.NpgsqlConnection(config.ConnectionString),
            _ => throw new NotSupportedException($"Database type {config.DbType} not supported.")
        };
    }
    public DatabaseType GetDatabaseType(string dbName)
    {
        return _configs.TryGetValue(dbName, out var config)
            ? config.DbType
            : throw new KeyNotFoundException($"Database configuration for '{dbName}' not found.");
    }
    public DatabaseConfig GetConfig(string databaseName)
    {
        return !_configs.ContainsKey(databaseName)
            ? throw new ArgumentException($"Database '{databaseName}' not found in configuration.")
            : _configs[databaseName];
    }
}
