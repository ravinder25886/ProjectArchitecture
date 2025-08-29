namespace RS.Dapper.Utility.Repositories.DapperRepository;
public interface IDapperRepository
{
    /// <summary>
    /// Inserts a new record into the specified table of the given database.
    /// </summary>
    /// <typeparam name="T">The type of the model representing the table structure.</typeparam>
    /// <param name="model">The object to insert.</param>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table where the record will be inserted.</param>
    /// <param name="keyColumn">The primary key column of the table (default is "Id").</param>
    /// <param name="transactionOn">
    /// If true, the operation will be executed inside a database transaction (commit/rollback handled automatically).
    /// If false, the operation executes normally without a transaction.
    /// </param>
    /// <returns>The number of rows affected or the newly generated Id depending on implementation.</returns>
    public Task<int> InsertAsync<T>(T model, string dbName, string tableName, string keyColumn = "Id", bool transactionOn = false);

    /// <summary>
    /// Updates an existing record in the specified table of the given database.
    /// </summary>
    /// <typeparam name="T">The type of the model representing the table structure.</typeparam>
    /// <param name="model">The object with updated values.</param>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table to update.</param>
    /// <param name="keyColumn">The primary key column used to identify the record (default is "Id").</param>
    /// <param name="transactionOn">
    /// If true, the operation will be executed inside a database transaction (commit/rollback handled automatically).
    /// If false, the operation executes normally without a transaction.
    /// </param>
    /// <returns>The number of rows affected.</returns>
    public Task<int> UpdateAsync<T>(T model, string dbName, string tableName, string keyColumn = "Id", bool transactionOn = false);

    /// <summary>
    /// Deletes a record from the specified table using its primary key.
    /// </summary>
    /// <param name="id">The value of the primary key of the record to delete.</param>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table from which to delete the record.</param>
    /// <param name="keyColumn">The primary key column used to identify the record (default is "Id").</param>
    /// <param name="transactionOn">
    /// If true, the operation will be executed inside a database transaction (commit/rollback handled automatically).
    /// If false, the operation executes normally without a transaction.
    /// </param>
    /// <returns>The number of rows affected.</returns>
    public Task<int> DeleteAsync(object id, string dbName, string tableName, string keyColumn = "Id", bool transactionOn = false);

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
    public Task<TModel?> GetByIdAsync<TModel, TId>(TId id, string dbName, string tableName, string keyColumn = "Id");

    /// <summary>
    /// Retrieves all records from the specified table.
    /// </summary>
    /// <typeparam name="T">The type of the model representing the table structure.</typeparam>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table to query.</param>
    /// <returns>An enumerable of all records in the table.</returns>
    public Task<IEnumerable<T>> GetAllAsync<T>(string dbName, string tableName);

    /// <summary>
    /// Retrieves paginated records from the specified table based on the provided paging and filtering options.
    /// </summary>
    /// <typeparam name="T">The type of the model representing the table structure.</typeparam>
    /// <param name="dbName">The name of the database. The system will use this to retrieve the connection string and database type from appsettings.</param>
    /// <param name="tableName">The name of the table to query.</param>
    /// <param name="pagedRequest">An object containing paging, filtering, and sorting options.</param>
    /// <returns>A PagedResult containing the requested page of records and pagination metadata.</returns>
    public Task<PagedResult<T>> GetPagedDataAsync<T>(string dbName, string tableName, PagedRequest pagedRequest);
}
