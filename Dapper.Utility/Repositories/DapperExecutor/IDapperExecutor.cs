using System.Data;

namespace RS.Dapper.Utility.Repositories.DapperExecutor;
public interface IDapperExecutor
{
   public Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.Text);
   public Task<T?> ExecuteScalarAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
   public Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
   public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.Text);
}
