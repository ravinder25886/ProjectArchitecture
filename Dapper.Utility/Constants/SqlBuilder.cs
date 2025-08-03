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
        return string.Join(", ", typeof(T).GetProperties()
            .Where(p => !Attribute.IsDefined(p, typeof(IgnoreParamAttribute)))
            .Select(p => Quote(p.Name, dbType)));
    }

    /// <summary>
    /// Builds a parameterized INSERT SQL statement with optional RETURNING/OUTPUT clause depending on database type.
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    /// <param name="model">Model instance</param>
    /// <param name="tableName">Table name, optionally with schema (e.g. "dbo.Users")</param>
    /// <param name="dbType">Database engine type</param>
    /// <param name="keyColumn">Primary key column name</param>
    /// <returns>SQL string</returns>
    public static string BuildInsert<T>(T model, string tableName, DatabaseType dbType, string keyColumn = "Id")
    {
        string cacheKey = $"Insert:{typeof(T).FullName}:{tableName}:{dbType}:{keyColumn}";

        return GetOrAddToCache(cacheKey, () =>
        {
            List<PropertyInfo> props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !string.Equals(p.Name, keyColumn, StringComparison.OrdinalIgnoreCase)
                            && !Attribute.IsDefined(p, typeof(IgnoreParamAttribute)))
                .ToList();

            string columns = string.Join(", ", props.Select(p => Quote(p.Name, dbType)));
            string parameters = string.Join(", ", props.Select(p => "@" + p.Name));
            string baseSql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

            return dbType switch
            {
                DatabaseType.SqlServer => $"{baseSql}; SELECT CAST(SCOPE_IDENTITY() AS INT);",
                DatabaseType.MySql => $"{baseSql}; SELECT LAST_INSERT_ID();",
                DatabaseType.PostgreSql => $"{baseSql} RETURNING {Quote(keyColumn, dbType)};",
                _ => throw new NotSupportedException($"Unsupported database type: {dbType}")
            };
        });
    }

    /// <summary>
    /// Builds an UPDATE SQL statement based on non-null properties except the key column.
    /// </summary>
    public static string BuildUpdate<T>(T model, string tableName, DatabaseType dbType, string keyColumn = "Id")
    {
        string cacheKey = $"Update:{typeof(T).FullName}:{tableName}:{dbType}:{keyColumn}";

        return GetOrAddToCache(cacheKey, () =>
        {
            List<PropertyInfo> props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !string.Equals(p.Name, keyColumn, StringComparison.OrdinalIgnoreCase)
                            && !Attribute.IsDefined(p, typeof(IgnoreParamAttribute)))
                .ToList();

            string setClause = string.Join(", ", props.Select(p => $"{Quote(p.Name, dbType)} = @{p.Name}"));
            return $"UPDATE {tableName} SET {setClause} WHERE {Quote(keyColumn, dbType)} = @{keyColumn};";
        });
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
    /// Builds a parameterized SQL COUNT query with dynamic WHERE filters, supporting caching of the SQL query template.
    /// </summary>
    /// <param name="tableName">The name of the database table to query.</param>
    /// <param name="filters">A list of filters specifying columns, operators, and values for the WHERE clause.</param>
    /// <param name="dbType">The type of the target database (e.g., SqlServer, MySql, PostgreSql) for proper SQL syntax quoting.</param>
    /// <returns>
    /// A tuple containing:
    ///   - The cached or newly generated SQL COUNT query string with parameter placeholders (cached by table name, filter columns, and operators).
    ///   - A new <see cref="DynamicParameters"/> instance with uniquely named parameters and their corresponding values for safe parameterization.
    /// </returns>
    /// <remarks>
    /// The SQL query string is cached based on table name, database type, and the filter columns and operators, but NOT on filter values.
    /// Filter values are always passed as fresh parameters to avoid cache collisions and ensure correct query execution.
    /// Parameter names in the cached SQL are generic (e.g., '@ColumnName'), while the actual <see cref="DynamicParameters"/> use unique names (e.g., '@Column_0', '@Column_1').
    /// This method ensures SQL injection safety by using parameterized queries.
    /// </remarks>
    public static (string sql, DynamicParameters parameters) BuildCountQueryWithFilters(
      string tableName,
      List<SqlFilter> filters,
      DatabaseType dbType)
    {
        string filtersKey = filters != null && filters.Any()
            ? string.Join(",", filters.Select(f => $"{f.Column}:{f.Operator}").OrderBy(s => s))
            : "nofilter";

        string cacheKey = $"CountQuery:{tableName}:{dbType}:{filtersKey}";

        // Get or build SQL string with generic parameter placeholders
        string sql = GetOrAddToCache(cacheKey, () =>
        {
            string Quote(string name)
            {
                return dbType switch
                {
                    DatabaseType.SqlServer => $"[{name}]",
                    DatabaseType.MySql => $"`{name}`",
                    DatabaseType.PostgreSql => $"\"{name}\"",
                    _ => name
                };
            }

            //string quotedTable = Quote(tableName);
            List<string> whereClauses = new();

            if (filters != null && filters.Any())
            {
                int paramIndex = 0;
                foreach (var filter in filters)
                {
                    string paramName = $"@{filter.Column}_{paramIndex}";
                    whereClauses.Add($"{Quote(filter.Column)} {filter.Operator.ToSqlString(dbType)} {paramName}");
                    paramIndex++;
                }
            }

            string whereClause = whereClauses.Count > 0
                ? " WHERE " + string.Join(" AND ", whereClauses)
                : "";

            return $"SELECT COUNT(1) FROM {tableName}{whereClause};";
        });

        // Build fresh DynamicParameters with unique param names per call
        DynamicParameters parameters = new DynamicParameters();
        if (filters != null && filters.Any())
        {
            for (int i = 0; i < filters.Count; i++)
            {
                var filter = filters[i];
                string paramName = $"@{filter.Column}_{i}"; // unique param name
                parameters.Add(paramName, filter.Value);
            }
        }

        return (sql, parameters);
    }

    /// <summary>
    /// Appends a paging clause (ORDER BY + OFFSET/FETCH or LIMIT/OFFSET) to any SQL query.
    /// </summary>
    /// <param name="sql">Base SQL query (should NOT contain ORDER BY or paging)</param>
    /// <param name="dbType">Database type</param>
    /// <param name="pageSize">Number of records per page</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="orderBy">Column name to order by (default "Id")</param>
    /// <returns>SQL with appended paging clause</returns>
    public static string AppendPaging(string sql, DatabaseType dbType, int pageSize, int pageNumber, string orderBy = "Id")
    {
        sql = sql.TrimEnd(';');
        int offset = (pageNumber - 1) * pageSize;
        string orderByQuoted = Quote(orderBy, dbType);

        string pagingSql = dbType switch
        {
            DatabaseType.SqlServer => $"{sql} ORDER BY {orderByQuoted} OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY",
            DatabaseType.MySql => $"{sql} ORDER BY {orderByQuoted} LIMIT {pageSize} OFFSET {offset}",
            DatabaseType.PostgreSql => $"{sql} ORDER BY {orderByQuoted} LIMIT {pageSize} OFFSET {offset}",
            _ => throw new NotSupportedException($"Unsupported database type: {dbType}")
        };
        return pagingSql;
    }
   

}
