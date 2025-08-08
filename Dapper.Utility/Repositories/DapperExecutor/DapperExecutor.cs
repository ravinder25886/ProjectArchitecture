using System.Data;
using Dapper;
using RS.Dapper.Utility.Connections;

namespace RS.Dapper.Utility.Repositories.DapperExecutor;
public class DapperExecutor(DapperContext context) : IDapperExecutor
{
    private readonly DapperContext _context = context;

    public async Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.Text)
    {
        using var _connection = _context.CreateConnection();
        return await _connection.ExecuteAsync(sql, param, commandType: commandType);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
    {
        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<T>(sql, param, commandType: commandType);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<T>(sql, param, commandType: commandType);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<T>(sql, param, commandType: commandType);
    }
}
