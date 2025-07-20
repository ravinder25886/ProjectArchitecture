using System.Reflection;

namespace ProjectName.DataAccess.Constants;
public enum DatabaseType
{
    SqlServer,
    MySql,
    PostgreSql
}

public static class SqlBuilder
{
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
        List<PropertyInfo> props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !string.Equals(p.Name, keyColumn, StringComparison.OrdinalIgnoreCase)
                        && p.GetValue(model) != null)
            .ToList();

        string columns = string.Join(", ", props.Select(p => p.Name));
        string parameters = string.Join(", ", props.Select(p => "@" + p.Name));

        string baseSql = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

        return dbType switch
        {
            DatabaseType.SqlServer => $"{baseSql}; SELECT CAST(SCOPE_IDENTITY() AS INT);",
            DatabaseType.MySql => $"{baseSql}; SELECT LAST_INSERT_ID();",
            DatabaseType.PostgreSql => $"{baseSql} RETURNING {keyColumn};",
            _ => throw new NotSupportedException($"Unsupported database type: {dbType}")
        };
    }

    /// <summary>
    /// Builds an UPDATE SQL statement based on non-null properties except the key column.
    /// </summary>
    public static string BuildUpdate<T>(T model, string tableName, string keyColumn = "Id")
    {
        List<PropertyInfo> props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => !string.Equals(p.Name, keyColumn, StringComparison.OrdinalIgnoreCase)
                        && p.GetValue(model) != null)
            .ToList();

        string setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));

        return $"UPDATE {tableName} SET {setClause} WHERE {keyColumn} = @{keyColumn};";
    }

    /// <summary>
    /// Builds a DELETE SQL statement.
    /// </summary>
    public static string BuildDelete(string tableName, string keyColumn = "Id")
    {
        return $"DELETE FROM {tableName} WHERE {keyColumn} = @{keyColumn};";
    }

    /// <summary>
    /// Builds a SELECT statement to get a record by primary key.
    /// </summary>
    public static string BuildSelectById(string tableName, string keyColumn = "Id")
    {
        return $"SELECT * FROM {tableName} WHERE {keyColumn} = @{keyColumn};";
    }

    /// <summary>
    /// Builds a SELECT statement to get all records.
    /// </summary>
    public static string BuildSelectAll(string tableName)
    {
        return $"SELECT * FROM {tableName};";
    }

    internal static string BuildInsert<T>(T? model, string tableName, global::DatabaseType databaseType, string keyColumn)
    {
        throw new NotImplementedException();
    }
}
