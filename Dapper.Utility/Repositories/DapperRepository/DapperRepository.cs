using Dapper;

using RS.Dapper.Utility.Constants;
using RS.Dapper.Utility.Resolver;

namespace RS.Dapper.Utility.Repositories.DapperRepository;
public class DapperRepository(IDatabaseResolver databaseResolver) : IDapperRepository
{
    private readonly IDatabaseResolver _databaseResolver = databaseResolver;

    /// <summary>
    /// Inserts a new record into the specified table of the given database.
    /// </summary>
    /// <typeparam name="T">The type of the model representing the table structure.</typeparam>
    /// <param name="model">The object to insert.</param>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table where the record will be inserted.</param>
    /// <param name="keyColumn">The primary key column of the table (default is "Id").</param>
    /// <returns>The number of rows affected or the newly generated Id depending on implementation.</returns>
    public async Task<int> InsertAsync<T>(T model, string dbName, string tableName, string keyColumn = "Id")
    {
        var (sql, parameters) = SqlBuilder.BuildInsert(model, tableName, _databaseResolver.GetDatabaseType(dbName), keyColumn);
        using var connection = _databaseResolver.GetConnection(dbName);
        return await connection.ExecuteScalarAsync<int>(sql, parameters);
    }

    /// <summary>
    /// Updates an existing record in the specified table of the given database.
    /// </summary>
    /// <typeparam name="T">The type of the model representing the table structure.</typeparam>
    /// <param name="model">The object with updated values.</param>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table to update.</param>
    /// <param name="keyColumn">The primary key column used to identify the record (default is "Id").</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> UpdateAsync<T>(T model, string dbName, string tableName, string keyColumn = "Id")
    {
        var (sql, parameters) = SqlBuilder.BuildUpdate(model, tableName, _databaseResolver.GetDatabaseType(dbName), keyColumn);
        using var connection = _databaseResolver.GetConnection(dbName);
        return await connection.ExecuteAsync(sql, parameters);
    }

    /// <summary>
    /// Deletes a record from the specified table using its primary key.
    /// </summary>
    /// <param name="id">The value of the primary key of the record to delete.</param>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table from which to delete the record.</param>
    /// <param name="keyColumn">The primary key column used to identify the record (default is "Id").</param>
    /// <returns>The number of rows affected.</returns>
    public async Task<int> DeleteAsync(object id, string dbName, string tableName, string keyColumn = "Id")
    {
        string sql = SqlBuilder.BuildDelete(tableName, _databaseResolver.GetDatabaseType(dbName), keyColumn);
        using var connection = _databaseResolver.GetConnection(dbName);
        return await connection.ExecuteAsync(sql, new { Id = id });
    }

    /// <summary>
    /// Retrieves a single record from the specified table by its primary key.
    /// </summary>
    /// <typeparam name="TModel">The type of the model representing the table structure.</typeparam>
    /// <typeparam name="TId">The type of the primary key.</typeparam>
    /// <param name="id">The primary key value of the record to retrieve.</param>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table to query.</param>
    /// <param name="keyColumn">The primary key column used to identify the record (default is "Id").</param>
    /// <returns>The record if found; otherwise, null.</returns>
    public async Task<TModel?> GetByIdAsync<TModel, TId>(TId id, string dbName, string tableName, string keyColumn = "Id")
    {
        string sql = SqlBuilder.BuildSelectById<TModel>(tableName, _databaseResolver.GetDatabaseType(dbName), keyColumn);
        using var connection = _databaseResolver.GetConnection(dbName);
        return await connection.QuerySingleOrDefaultAsync<TModel>(sql, new { Id = id });
    }

    /// <summary>
    /// Retrieves all records from the specified table.
    /// </summary>
    /// <typeparam name="T">The type of the model representing the table structure.</typeparam>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table to query.</param>
    /// <returns>An enumerable of all records in the table.</returns>
    public async Task<IEnumerable<T>> GetAllAsync<T>(string dbName, string tableName)
    {
        string sql = SqlBuilder.BuildSelectAll<T>(tableName, _databaseResolver.GetDatabaseType(dbName));
        using var connection = _databaseResolver.GetConnection(dbName);
        return await connection.QueryAsync<T>(sql);
    }
    /// <summary>
    /// Retrieves paginated records from the specified table based on the provided paging and filtering options.
    /// </summary>
    /// <typeparam name="T">The type of the model representing the table structure.</typeparam>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table to query.</param>
    /// <param name="pagedRequest">An object containing paging, filtering, and sorting options.</param>
    /// <returns>A PagedResult containing the requested page of records and pagination metadata.</returns>
    public async Task<PagedResult<T>> GetPagedDataAsync<T>(string dbName, string tableName, PagedRequest pagedRequest)
    {
        List<SqlFilter> filters = new List<SqlFilter>();
        foreach (var item in pagedRequest.Filters)
        {
            if (SqlFilterHelper.IsValidFilterValue(item.Value))
            {
                filters.Add(item);
            }
        }
        //Build a SQL query for paging data
        var (sql, parameters) = SqlBuilder.BuildCountAndDataQueryWithParams<T>(
            tableName,
            filters,
            _databaseResolver.GetDatabaseType(dbName),
            pageSize: pagedRequest.PageSize,
            pageNumber: pagedRequest.PageNumber,
            orderBy: pagedRequest.OrderBy,
            sortDirection: pagedRequest.SortDirection
        );
        using var connection = _databaseResolver.GetConnection(dbName);
        using var multi = await connection.QueryMultipleAsync(sql, parameters);
        // Then use both with Dapper
        int totalRecords = await multi.ReadFirstAsync<int>();
        List<T> data = (await multi.ReadAsync<T>()).ToList();

        PagedResult<T> pagedResult = new PagedResult<T>
        {
            Items = (List<T>)data,
            PageNumber = pagedRequest.PageNumber,
            PageSize = pagedRequest.PageSize,
            TotalRecords = totalRecords
        };
        return pagedResult;
    }
}
