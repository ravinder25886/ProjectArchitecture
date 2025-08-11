using System.Data;
using System.Reflection;

using Dapper;

using Microsoft.Extensions.Caching.Memory;

using RS.Dapper.Utility.Attributes;

namespace RS.Dapper.Utility.Constants;
public static class SqlBuilder
{
    /*
     * If we don't want to use IMemoryCache then please un-comment following code and commment IMemoryCache code
    private static readonly ConcurrentDictionary<string, string> _sqlCache = new();
    /// <summary>
    /// Adds or retrieves a SQL query from the cache based on a key.
    /// </summary>
    private static string GetOrAddToCache(string cacheKey, Func<string> sqlBuilder)
    {
        return _sqlCache.GetOrAdd(cacheKey, _ => sqlBuilder());
    }
    */
    private static IMemoryCache? _cache;

    // Call this method once during app startup to inject IMemoryCache
    public static void Initialize(IMemoryCache memoryCache)
    {
        _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }
    private static string GetOrAddToCache(string cacheKey, Func<string> sqlBuilder)
    {
        return _cache == null
            ? throw new InvalidOperationException("SqlBuilder cache not initialized. Call SqlBuilder.Initialize() with IMemoryCache.")
            : _cache.GetOrCreate(cacheKey, entry =>
            sqlBuilder())!;
    }
    public static string QuoteIdentifier(string identifier, DatabaseType dbType)
    {
        return dbType switch
        {
            DatabaseType.SqlServer => $"[{identifier}]",
            DatabaseType.MySql => $"`{identifier}`",
            DatabaseType.PostgreSql => $"\"{identifier}\"",
            _ => throw new NotSupportedException($"Unsupported database type: {dbType}")
        };
    }
    /// <summary>
    /// Quotes a database identifier (table or column) based on the database type.
    /// </summary>
    private static string Quote(string name, DatabaseType dbType)
    {
        return dbType switch
        {
            DatabaseType.SqlServer => $"[{name}]",
            DatabaseType.MySql => $"`{name}`",
            DatabaseType.PostgreSql => $"\"{name}\"",
            _ => name,
        };
    }
    /// <summary>
    /// Gets a comma-separated, quoted list of columns from the model type.
    /// </summary>
    private static string GetColumnList<T>(DatabaseType dbType)
    {

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
          .Where(p => !Attribute.IsDefined(p, typeof(IgnoreParamAttribute)));

        var columns = props.Select(p =>
        {
            var attr = p.GetCustomAttribute<SqlParamAttribute>();
            string column = attr?.Name ?? p.Name;
            string property = p.Name;
            return $"{Quote(column, dbType)} AS {Quote(property, dbType)}";
        });

        return string.Join(", ", columns);
    }

    /// <summary>
    /// Builds a parameterized SQL INSERT statement for the given model and table,
    /// including all properties except the primary key (by default, "Id").
    /// Automatically maps property names to database column names based on the <c>[SqlParam]</c> attribute, if available.
    /// </summary>
    /// <typeparam name="T">The type of the model to insert.</typeparam>
    /// <param name="model">The instance of the model containing values to insert.</param>
    /// <param name="tableName">The name of the database table to insert into.</param>
    /// <param name="dbType">The target database type (e.g., SQL Server, MySQL, PostgreSQL).</param>
    /// <param name="keyColumn">The name of the primary key column to exclude from the insert (default is "Id").</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    ///   <item><description><c>Sql</c>: The generated SQL INSERT command as a string.</description></item>
    ///   <item><description><c>Parameters</c>: The Dapper <see cref="DynamicParameters"/> object with mapped values.</description></item>
    /// </list>
    /// </returns>
    public static (string Sql, DynamicParameters Parameters) BuildInsert<T>(
      T model,
      string tableName,
      DatabaseType dbType,
      string keyColumn = "Id")
    {
        string cacheKey = $"Insert:{typeof(T).FullName}:{tableName}:{dbType}:{keyColumn}";

        string sql = GetOrAddToCache(cacheKey, () =>
        {
            List<PropertyInfo> props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !string.Equals(p.Name, keyColumn, StringComparison.OrdinalIgnoreCase)
                            && !Attribute.IsDefined(p, typeof(IgnoreParamAttribute)))
                .ToList();

            string columns = string.Join(", ", props.Select(p =>
            {
                var attr = p.GetCustomAttribute<SqlParamAttribute>();
                return Quote(attr?.Name ?? p.Name, dbType);
            }));

            string parameters = string.Join(", ", props.Select(p =>
            {
                var attr = p.GetCustomAttribute<SqlParamAttribute>();
                return "@" + (attr?.Name ?? p.Name);
            }));

            string baseSql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

            return dbType switch
            {
                DatabaseType.SqlServer => $"{baseSql}; SELECT CAST(SCOPE_IDENTITY() AS INT);",
                DatabaseType.MySql => $"{baseSql}; SELECT LAST_INSERT_ID();",
                DatabaseType.PostgreSql => $"{baseSql} RETURNING {Quote(keyColumn, dbType)};",
                _ => throw new NotSupportedException($"Unsupported database type: {dbType}")
            };
        });

        // Build parameters using [SqlParam] if available
        DynamicParameters dynParams = new DynamicParameters();
        foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (Attribute.IsDefined(prop, typeof(IgnoreParamAttribute)) ||
                string.Equals(prop.Name, keyColumn, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var attr = prop.GetCustomAttribute<SqlParamAttribute>();
            string paramName = attr?.Name ?? prop.Name;
            object? value = prop.GetValue(model);
            dynParams.Add("@" + paramName, value);
        }

        return (sql, dynParams);
    }


    /// <summary>
    /// Builds a parameterized SQL UPDATE statement for the given model and table,
    /// updating all properties except the primary key (by default, "Id").
    /// Automatically maps property names to database column names based on the <c>[SqlParam]</c> attribute, if available.
    /// </summary>
    /// <typeparam name="T">The type of the model to update.</typeparam>
    /// <param name="model">The instance of the model containing updated values.</param>
    /// <param name="tableName">The name of the database table to update.</param>
    /// <param name="dbType">The target database type (e.g., SQL Server, MySQL, PostgreSQL).</param>
    /// <param name="keyColumn">The name of the primary key column used in the WHERE clause (default is "Id").</param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    ///   <item><description><c>Sql</c>: The generated SQL UPDATE command as a string.</description></item>
    ///   <item><description><c>Parameters</c>: The Dapper <see cref="DynamicParameters"/> object with mapped values, including the key column.</description></item>
    /// </list>
    /// </returns>
    public static (string Sql, DynamicParameters Parameters) BuildUpdate<T>(
    T model,
    string tableName,
    DatabaseType dbType,
    string keyColumn = "Id")
    {
        string cacheKey = $"Update:{typeof(T).FullName}:{tableName}:{dbType}:{keyColumn}";

        string sql = GetOrAddToCache(cacheKey, () =>
        {
            List<PropertyInfo> props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !Attribute.IsDefined(p, typeof(IgnoreParamAttribute)) &&
                            !string.Equals(p.Name, keyColumn, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // Generate SET clauses using column names
            string setClause = string.Join(", ", props.Select(p =>
            {
                var attr = p.GetCustomAttribute<SqlParamAttribute>();
                string columnName = attr?.Name ?? p.Name;
                return $"{Quote(columnName, dbType)} = @{columnName}";
            }));

            string whereClause = $"{Quote(keyColumn, dbType)} = @{keyColumn}";

            return $"UPDATE {tableName} SET {setClause} WHERE {whereClause};";
        });

        // Generate parameters
        DynamicParameters parameters = new DynamicParameters();
        foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (Attribute.IsDefined(prop, typeof(IgnoreParamAttribute)))
            {
                continue;
            }

            var attr = prop.GetCustomAttribute<SqlParamAttribute>();
            string paramName = attr?.Name ?? prop.Name;
            object? value = prop.GetValue(model);
            parameters.Add("@" + paramName, value);
        }

        return (sql, parameters);
    }


    /// <summary>
    /// Builds a DELETE SQL statement.
    /// </summary>
    public static string BuildDelete(string tableName, DatabaseType dbType, string keyColumn = "Id")
    {
        string cacheKey = $"Delete:{tableName}:{dbType}:{keyColumn}";

        return GetOrAddToCache(cacheKey, () =>
            $"DELETE FROM {tableName} WHERE {Quote(keyColumn, dbType)} = @{keyColumn};"
        );
    }

    /// <summary>
    /// Builds a SELECT statement to get a record by primary key.
    /// </summary>
    public static string BuildSelectById<T>(string tableName, DatabaseType dbType, string keyColumn = "Id")
    {
        string cacheKey = $"SelectById:{typeof(T).FullName}:{tableName}:{dbType}:{keyColumn}";

        return GetOrAddToCache(cacheKey, () =>
        {
            string columns = GetColumnList<T>(dbType);
            return $"SELECT {columns} FROM {tableName} WHERE {Quote(keyColumn, dbType)} = @{keyColumn};";
        });
    }

    /// <summary>
    /// Builds a SELECT statement to get all records.
    /// </summary>
    public static string BuildSelectAll<T>(string tableName, DatabaseType dbType)
    {
        string cacheKey = $"SelectAll:{typeof(T).FullName}:{tableName}:{dbType}";

        return GetOrAddToCache(cacheKey, () =>
        {
            string columns = GetColumnList<T>(dbType);
            return $"SELECT {columns} FROM {tableName};";
        });
    }

    /// <summary>
    /// Builds a SELECT query including only properties not marked with [IgnoreParam].
    /// </summary>
    public static string BuildSelectColumns<T>(string tableName)
    {
        string cacheKey = $"SelectColumns:{typeof(T).FullName}:{tableName}";
        return GetOrAddToCache(cacheKey, () =>
        {
            var columns = typeof(T).GetProperties()
                .Where(p => !Attribute.IsDefined(p, typeof(IgnoreParamAttribute)))
                .Select(p => p.Name);
            return $"SELECT {string.Join(", ", columns)} FROM {tableName};";
        });
    }

    /// <summary>
    /// Builds a SELECT All query with WHERE clause filters on specific columns no paging.
    /// </summary>
    public static string BuildSelectWithFilters<T>(string tableName, DatabaseType dbType, IEnumerable<string> filterColumns)
    {
        string filterKey = string.Join(",", filterColumns.OrderBy(x => x));
        string cacheKey = $"SelectWithFilters:{typeof(T).FullName}:{tableName}:{dbType}:{filterKey}";

        return GetOrAddToCache(cacheKey, () =>
        {
            string columns = GetColumnList<T>(dbType);
            string whereClause = string.Join(" AND ", filterColumns.Select(c => $"{Quote(c, dbType)} = @{c}"));
            return $"SELECT {columns} FROM {tableName} WHERE {whereClause};";
        });
    }

    /// <summary>
    /// Builds a SELECT All query using LIKE search across one or more columns.
    /// </summary>
    public static string BuildSearchQuery<T>(string tableName, DatabaseType dbType, IEnumerable<string> searchColumns)
    {
        string filterKey = string.Join(",", searchColumns.OrderBy(x => x));
        string cacheKey = $"SearchQuery:{typeof(T).FullName}:{tableName}:{dbType}:{filterKey}";

        return GetOrAddToCache(cacheKey, () =>
        {
            string columns = GetColumnList<T>(dbType);

            // Build LIKE with concat: use CONCAT for SQL Server/MySQL, || for PostgreSQL
            string concatValue = dbType switch
            {
                DatabaseType.PostgreSql => $"'%' || @{searchColumns.First()} || '%'", // for one param (adjust below)
                _ => $"CONCAT('%', @{searchColumns.First()}, '%')"
            };

            // For multiple columns, generate individual conditions with their own param names
            var likeClauses = searchColumns.Select(c =>
            {
                string paramName = c;
                string likeExpr = dbType switch
                {
                    DatabaseType.PostgreSql => $"{Quote(c, dbType)} ILIKE '%' || @{paramName} || '%'",
                    _ => $"{Quote(c, dbType)} LIKE CONCAT('%', @{paramName}, '%')"
                };
                return likeExpr;
            });

            string whereClause = string.Join(" OR ", likeClauses);

            return $"SELECT {columns} FROM {tableName} WHERE {whereClause};";
        });
    }


    /// <summary>
    /// Builds a paged SELECT query with OFFSET and FETCH based on page size and number.
    /// </summary>
    public static string BuildPagedQuery<T>(string tableName, DatabaseType dbType, int pageSize, int pageNumber, string orderBy = "Id")
    {
        string cacheKey = $"PagedQuery:{typeof(T).FullName}:{tableName}:{dbType}:{orderBy}:{pageSize}:{pageNumber}";

        return GetOrAddToCache(cacheKey, () =>
        {
            string columns = GetColumnList<T>(dbType);
            int offset = (pageNumber - 1) * pageSize;

            return dbType switch
            {
                DatabaseType.SqlServer => $"SELECT {columns} FROM {tableName} ORDER BY {Quote(orderBy, dbType)} OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY;",
                DatabaseType.MySql => $"SELECT {columns} FROM {tableName} ORDER BY {Quote(orderBy, dbType)} LIMIT {pageSize} OFFSET {offset};",
                DatabaseType.PostgreSql => $"SELECT {columns} FROM {tableName} ORDER BY {Quote(orderBy, dbType)} LIMIT {pageSize} OFFSET {offset};",
                _ => throw new NotSupportedException($"Unsupported database type: {dbType}")
            };
        });
    }


    /// <summary>
    /// Builds a SELECT query using an IN clause for a list of values.
    /// </summary>
    public static string BuildSelectIn<T>(string tableName, DatabaseType dbType, string columnName)
    {
        string cacheKey = $"SelectIn:{typeof(T).FullName}:{tableName}:{dbType}:{columnName}";

        return GetOrAddToCache(cacheKey, () =>
        {
            string columns = GetColumnList<T>(dbType);
            return $"SELECT {columns} FROM {tableName} WHERE {Quote(columnName, dbType)} IN @Values;";
        });
    }
    /// <summary>
    /// Builds a SELECT query with dynamic WHERE filters and returns both SQL and DynamicParameters based on database type.
    /// </summary>
    public static (string Sql, DynamicParameters Parameters) BuildDynamicFilterQueryWithParams<T>(
       string tableName,
       List<SqlFilter>? filters,
       DatabaseType dbType,
       int pageSize = 10,
       int pageNumber = 1,
       string? orderBy = "Id",
       string sortDirection = "ASC"
   )
    {
        // Prepare cache key based on tableName, dbType, and filters' column+operator only
        string filtersKey = filters != null && filters.Any()
            ? string.Join(",", filters.Select(f => $"{f.Column}:{f.Operator}").OrderBy(s => s))
            : "nofilter";

        string cacheKey = $"DynamicFilterQuery:{typeof(T).FullName}:{tableName}:{dbType}:{filtersKey}:{orderBy}:{sortDirection}:{pageSize}:{pageNumber}";

        // Use GetOrAddToCache to get or build the SQL string (no params here)
        string sql = GetOrAddToCache(cacheKey, () =>
        {
            List<string> whereClauses = new List<string>();

            if (filters != null && filters.Any())
            {
                int paramIndex = 0;
                foreach (var filter in filters)
                {
                    // Use a placeholder param name in SQL; actual param names will differ below

                    whereClauses.Add($"{QuoteIdentifier(filter.Column, dbType)} {filter.Operator.ToSqlString(dbType)} @{filter.Column}_{paramIndex++}");
                }
            }

            string whereClause = whereClauses.Count > 0
                ? "WHERE " + string.Join(" AND ", whereClauses)
                : "";

            if (string.IsNullOrEmpty(orderBy) && dbType == DatabaseType.SqlServer)
            {
                orderBy = "Id";
            }

            string orderClause = !string.IsNullOrEmpty(orderBy)
                ? $"ORDER BY {QuoteIdentifier(orderBy, dbType)} {sortDirection.ToUpper()}"
                : "";

            int offset = (pageNumber - 1) * pageSize;

            string paginationClause = dbType switch
            {
                DatabaseType.SqlServer => $"OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY",
                DatabaseType.MySql => $"LIMIT {pageSize} OFFSET {offset}",
                DatabaseType.PostgreSql => $"LIMIT {pageSize} OFFSET {offset}",
                _ => throw new NotSupportedException($"Unsupported database type: {dbType}")
            };

            string columns = GetColumnList<T>(dbType);

            return $"SELECT {columns} FROM {tableName} {whereClause} {orderClause} {paginationClause};";
        });

        // Build fresh parameters with unique names for this call (to avoid conflicts)
        DynamicParameters parameters = new DynamicParameters();
        if (filters != null && filters.Any())
        {
            int paramIndex = 0;
            foreach (var filter in filters)
            {
                string paramName = $"@{filter.Column}_{paramIndex++}";
                parameters.Add(paramName, filter.Value);
            }
        }

        return (sql, parameters);
    }
    /// <summary>
    /// Builds a single SQL query that retrieves both the total record count and a paginated data set
    /// based on dynamic WHERE filters, using parameterized queries for SQL injection safety.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the entity being queried. Used for determining the column list via <c>GetColumnList&lt;T&gt;</c>.
    /// </typeparam>
    /// <param name="tableName">
    /// Name of the target database table.
    /// </param>
    /// <param name="filters">
    /// Optional list of <see cref="SqlFilter"/> objects specifying column, operator, and value for filtering.
    /// If null or empty, no WHERE clause will be added.
    /// </param>
    /// <param name="dbType">
    /// The type of the target database (<see cref="DatabaseType.SqlServer"/>, <see cref="DatabaseType.MySql"/>, <see cref="DatabaseType.PostgreSql"/>).
    /// Determines correct identifier quoting and pagination syntax.
    /// </param>
    /// <param name="pageSize">
    /// The number of records to return per page. Defaults to 10.
    /// </param>
    /// <param name="pageNumber">
    /// The page number to retrieve (1-based index). Defaults to 1.
    /// </param>
    /// <param name="orderBy">
    /// Column name to order the results by. Defaults to "Id" for SQL Server if not specified.
    /// </param>
    /// <param name="sortDirection">
    /// Sort direction: "ASC" or "DESC". Defaults to "ASC".
    /// </param>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item>
    /// <description><c>Sql</c> — The full SQL statement containing two SELECT queries: one for total count, and one for paginated data.</description>
    /// </item>
    /// <item>
    /// <description><c>Parameters</c> — A <see cref="DynamicParameters"/> object containing all parameter values for both queries.</description>
    /// </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method combines two queries (COUNT + data) into a single SQL command, allowing both results to be
    /// retrieved in one database round trip using <c>QueryMultiple</c> in Dapper.
    /// </para>
    /// <para>
    /// Caching is applied to the generated SQL string based on table name, filters (columns + operators), ordering,
    /// sort direction, page size, and page number. Filter values are excluded from the cache key.
    /// </para>
    /// </remarks>
    public static (string Sql, DynamicParameters Parameters) BuildCountAndDataQueryWithParams<T>(
     string tableName,
     List<SqlFilter>? filters,
     DatabaseType dbType,
     int pageSize = 10,
     int pageNumber = 1,
     string? orderBy = "Id",
     string sortDirection = "ASC"
 )
    {
        string filtersKey = filters != null && filters.Any()
            ? string.Join(",", filters.Select(f => $"{f.Column}:{f.Operator}").OrderBy(s => s))
            : "nofilter";

        string cacheKey = $"CountAndData:{typeof(T).FullName}:{tableName}:{dbType}:{filtersKey}:{orderBy}:{sortDirection}:{pageSize}:{pageNumber}";

        string sql = GetOrAddToCache(cacheKey, () =>
        {
            List<string> whereClauses = new();

            if (filters != null && filters.Any())
            {
                int paramIndex = 0;
                foreach (var filter in filters)
                {
                    whereClauses.Add($"{QuoteIdentifier(filter.Column, dbType)} {filter.Operator.ToSqlString(dbType)} @{filter.Column}_{paramIndex++}");
                }
            }

            string whereClause = whereClauses.Count > 0
                ? "WHERE " + string.Join(" AND ", whereClauses)
                : "";

            if (string.IsNullOrEmpty(orderBy) && dbType == DatabaseType.SqlServer)
            {
                orderBy = "Id";
            }

            string orderClause = !string.IsNullOrEmpty(orderBy)
                ? $"ORDER BY {QuoteIdentifier(orderBy, dbType)} {sortDirection.ToUpper()}"
                : "";

            int offset = (pageNumber - 1) * pageSize;

            string paginationClause = dbType switch
            {
                DatabaseType.SqlServer => $"OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY",
                DatabaseType.MySql => $"LIMIT {pageSize} OFFSET {offset}",
                DatabaseType.PostgreSql => $"LIMIT {pageSize} OFFSET {offset}",
                _ => throw new NotSupportedException($"Unsupported database type: {dbType}")
            };

            string columns = GetColumnList<T>(dbType);

            // Return both count and data in one call
            return $@"
            SELECT COUNT(1) 
            FROM {tableName} {whereClause};

            SELECT {columns} 
            FROM {tableName} {whereClause} {orderClause} {paginationClause};
        ";
        });

        DynamicParameters parameters = new();
        if (filters != null && filters.Any())
        {
            int paramIndex = 0;
            foreach (var filter in filters)
            {
                string paramName = $"@{filter.Column}_{paramIndex++}";
                parameters.Add(paramName, filter.Value);
            }
        }

        return (sql, parameters);
    }

}
