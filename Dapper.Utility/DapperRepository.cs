using System.Data;
using Dapper;
using RS.Dapper.Utility.Connections;
using RS.Dapper.Utility.Constants;

namespace RS.Dapper.Utility;
public class DapperRepository(DapperContext context): IDapperRepository
{
    private readonly DapperContext _context = context;
    private readonly DatabaseType _databaseType = context.DbType;
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
        var (sql, parameters) = SqlBuilder.BuildInsert(model, tableName, _databaseType, keyColumn);
        return await _connection.ExecuteScalarAsync<int>(sql, parameters);
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
        var (sql, parameters) = SqlBuilder.BuildUpdate(model, tableName, _databaseType,keyColumn);
        return await _connection.ExecuteAsync(sql, parameters);
    }

    /// <summary>
    /// Deletes a record from the specified table based on the primary key value.
    /// </summary>
    /// <param name="tableName">The name of the target table (with schema if needed).</param>
    /// <param name="id">The primary key value of the record to delete.</param>
    /// <param name="keyColumn">The primary key column name (default is "Id").</param>
    /// <returns>The number of rows affected by the delete operation.</returns>
    public async Task<int> DeleteAsync(object id, string tableName, string keyColumn = "Id")
    {
        string sql = SqlBuilder.BuildDelete(tableName, _databaseType, keyColumn);
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
    public async Task<TModel?> GetByIdAsync<TModel, TId>(TId id, string tableName, string keyColumn = "Id")
    {
        string sql = SqlBuilder.BuildSelectById< TModel>(tableName, _databaseType, keyColumn);
        return await _connection.QuerySingleOrDefaultAsync<TModel>(sql, new { Id = id });
    }

    /// <summary>
    /// Retrieves all records from the specified table.
    /// </summary>
    /// <typeparam name="T">The model type representing the table structure.</typeparam>
    /// <param name="tableName">The name of the target table (with schema if needed).</param>
    /// <returns>An enumerable of all records in the table.</returns>
    public async Task<IEnumerable<T>> GetAllAsync<T>(string tableName)
    {
        string sql = SqlBuilder.BuildSelectAll<T>(tableName, _databaseType);
        return await _connection.QueryAsync<T>(sql);
    }

    public async Task<PagedResult<T>> GetPagedDataAsync<T>(string tableName, PagedRequest pagedRequest)
    {
        List<SqlFilter> filters = new List<SqlFilter>();
        foreach (var item in pagedRequest.Filters)
        {
            if (SqlFilterHelper.IsValidFilterValue(item.Value))
            {
                filters.Add(item);
            }
        }

        var (pagedSql, parameters) = SqlBuilder.BuildDynamicFilterQueryWithParams<T>(
            tableName: tableName,
            filters: filters,
            dbType: _databaseType,
            pageSize: pagedRequest.PageSize,
            pageNumber: pagedRequest.PageNumber,
            orderBy: pagedRequest.OrderBy,
            sortDirection: pagedRequest.SortDirection 
        );

        // 2. Get count SQL
         var (countSql, countParameters) = SqlBuilder.BuildCountQueryWithFilters(
            tableName: tableName,
            filters: filters,
            dbType: _databaseType
        );

        // Then use both with Dapper
        int totalRecords =await _connection.ExecuteScalarAsync<int>(countSql, countParameters);
        var data =await _connection.QueryAsync<T>(pagedSql, parameters);

        PagedResult<T> pagedResult = new PagedResult<T>
        {
            Items= (List<T>)data,
            PageNumber= pagedRequest.PageNumber,
            PageSize= pagedRequest.PageSize,
            TotalRecords=totalRecords
        };
        return pagedResult;
    }
}
