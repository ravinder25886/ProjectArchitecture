using System.Data;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ProjectName.DataAccess.Connections;

public class DapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public DapperContext(IConfiguration configuration)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        _configuration = configuration;
#pragma warning disable CS8601 // Possible null reference assignment.
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
