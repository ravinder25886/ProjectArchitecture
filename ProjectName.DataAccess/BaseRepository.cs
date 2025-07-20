using System.Data;
using Dapper;
using ProjectName.DataAccess.Connections;
using ProjectName.DataAccess.Constants;

public abstract class BaseRepository(DapperContext context)
{
    private readonly DapperContext _context = context;
    private readonly DatabaseType _databaseType=context.DbType;
    // Reusable connection property
    protected IDbConnection _connection => _context.CreateConnection();
 /// <summary>
/// Inserts a new record into the specified table and returns the newly generated primary key.
/// </summary>
/// <typeparam name="T">The model type representing the table structure.</typeparam>
/// <param name="model">The model instance to insert.</param>
/// <param name="tableName">The name of the target table (with schema if needed).</param>
/// <param name="keyColumn">The primary key column name (default is "Id").</param>
/// <returns>The primary key value of the newly inserted record.</returns>
public async Task<int> InsertAsync<T>(T model, string tableName, string keyColumn = "Id")
    {
        string sql = SqlBuilder.BuildInsert(model, tableName, _databaseType, keyColumn);
        // Executes the insert and returns the generated primary key.
        return await _connection.ExecuteScalarAsync<int>(sql, model);
    }

    /// <summary>
    /// Updates an existing record in the specified table based on the primary key.
    /// </summary>
    /// <typeparam name="T">The model type representing the table structure.</typeparam>
    /// <param name="model">The model instance with updated values (must include the primary key).</param>
    /// <param name="tableName">The name of the target table (with schema if needed).</param>
    /// <param name="keyColumn">The primary key column name (default is "Id").</param>
    /// <returns>The number of rows affected by the update.</returns>
    public async Task<int> UpdateAsync<T>(T model, string tableName, string keyColumn = "Id")
    {
        string sql = SqlBuilder.BuildUpdate(model, tableName, keyColumn);
        return await _connection.ExecuteAsync(sql, model);
    }

    /// <summary>
    /// Deletes a record from the specified table based on the primary key value.
    /// </summary>
    /// <param name="tableName">The name of the target table (with schema if needed).</param>
    /// <param name="id">The primary key value of the record to delete.</param>
    /// <param name="keyColumn">The primary key column name (default is "Id").</param>
    /// <returns>The number of rows affected by the delete operation.</returns>
    public async Task<int> DeleteAsync(string tableName, int id, string keyColumn = "Id")
    {
        string sql = SqlBuilder.BuildDelete(tableName, keyColumn);
        return await _connection.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Retrieves a single record by primary key from the specified table.
    /// </summary>
    /// <typeparam name="T">The model type representing the table structure.</typeparam>
    /// <param name="id">The primary key value of the record to retrieve.</param>
    /// <param name="tableName">The name of the target table (with schema if needed).</param>
    /// <param name="keyColumn">The primary key column name (default is "Id").</param>
    /// <returns>The model instance if found; otherwise, null.</returns>
    public async Task<T?> GetByIdAsync<T>(int id, string tableName, string keyColumn = "Id")
    {
        string sql = SqlBuilder.BuildSelectById(tableName, keyColumn);
        return await _connection.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
    }

    /// <summary>
    /// Retrieves all records from the specified table.
    /// </summary>
    /// <typeparam name="T">The model type representing the table structure.</typeparam>
    /// <param name="tableName">The name of the target table (with schema if needed).</param>
    /// <returns>An enumerable of all records in the table.</returns>
    public async Task<IEnumerable<T>> GetAllAsync<T>(string tableName)
    {
        string sql = SqlBuilder.BuildSelectAll(tableName);
        return await _connection.QueryAsync<T>(sql);
    }

}
