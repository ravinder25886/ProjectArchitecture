using System.Collections.Concurrent;
using System.Data;
using System.Reflection;

using Dapper;

using RS.Dapper.Utility.Attributes;

namespace RS.Dapper.Utility.Constants;
public static class SqlBuilder
{
    private static readonly ConcurrentDictionary<string, string> _sqlCache = new();
    /// <summary>
    /// Adds or retrieves a SQL query from the cache based on a key.
    /// </summary>
    private static string GetOrAddToCache(string cacheKey, Func<string> sqlBuilder)
    {
        return _sqlCache.GetOrAdd(cacheKey, _ => sqlBuilder());
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
     string sortDirection = "ASC" // NEW PARAM
 )
    {
        DynamicParameters parameters = new DynamicParameters();
        List<string> whereClauses = new List<string>();

        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                string paramName = $"@{filter.Column}_{parameters.ParameterNames.Count()}";
                whereClauses.Add($"{QuoteIdentifier(filter.Column, dbType)} {filter.Operator.ToSqlString(dbType)} {paramName}");
                parameters.Add(paramName, filter.Value);
            }
        }

        string whereClause = whereClauses.Count > 0
            ? "WHERE " + string.Join(" AND ", whereClauses)
            : "";
        // If orderBy is null/empty and DB is SqlServer, use default column (e.g. Id)
        if (string.IsNullOrEmpty(orderBy) && dbType == DatabaseType.SqlServer)
        {
            orderBy = "Id";  // Or your actual primary key column
        }

        string orderClause = !string.IsNullOrEmpty(orderBy)
            ? $"ORDER BY {QuoteIdentifier(orderBy, dbType)} {sortDirection?.ToUpper() ?? "ASC"}"
            : "";

        int offset = (pageNumber - 1) * pageSize;

        string paginationClause = dbType switch
        {
            DatabaseType.SqlServer => $"OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY",
            DatabaseType.MySql => $"LIMIT {pageSize} OFFSET {offset}",
            DatabaseType.PostgreSql => $"LIMIT {pageSize} OFFSET {offset}",
            _ => throw new NotSupportedException($"Unsupported database type: {dbType}")
        };

        string sql = $"SELECT * FROM {tableName} {whereClause} {orderClause} {paginationClause};";
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
    public static (string sql, DynamicParameters parameters) BuildCountQueryWithFilters(
    string tableName,
    List<SqlFilter> filters,
    DatabaseType dbType)
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

        string quotedTable = Quote(tableName);
        List<string> whereClauses = new();
        DynamicParameters parameters = new DynamicParameters();

        for (int i = 0; i < filters.Count; i++)
        {
            var filter = filters[i];
            string paramName = $"@{filter.Column}_{i}"; // Unique
            whereClauses.Add($"{Quote(filter.Column)} {filter.Operator.ToSqlString(dbType)} {paramName}");
            parameters.Add(paramName, filter.Value);
        }

        string whereClause = whereClauses.Count > 0
            ? " WHERE " + string.Join(" AND ", whereClauses)
            : "";

        string sql = $"SELECT COUNT(1) FROM {quotedTable}{whereClause};";

        return (sql, parameters);
    }

}
