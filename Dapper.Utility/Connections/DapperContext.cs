using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Npgsql;

using RS.Dapper.Utility.Constants;

namespace RS.Dapper.Utility.Connections;

public class DapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    public DatabaseType DbType { get; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public DapperContext(IConfiguration configuration)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    {
        _configuration = configuration;
#pragma warning disable CS8601 // Possible null reference assignment.
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
#pragma warning restore CS8601 // Possible null reference assignment.
                              // Parse the database type from config, default to SqlServer if invalid
        string? dbTypeString = configuration["DatabaseSettings:Type"];
        DbType = Enum.TryParse(dbTypeString, ignoreCase: true, out DatabaseType dt)
            ? dt
            : DatabaseType.SqlServer;
    }

    //public IDbConnection CreateConnection()
    //{
    //    return new SqlConnection(_connectionString);
    //}

    //Please use this Merhods when you are planing to switch your project database
    public IDbConnection CreateConnection()
    {
        return DbType switch
        {
            //DatabaseType.MySql => new MySqlConnection(_connectionString),
            DatabaseType.PostgreSql => new NpgsqlConnection(_connectionString),
            _ => new SqlConnection(_connectionString),
        };
    }
}
