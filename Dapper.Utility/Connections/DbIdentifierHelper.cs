using RS.Dapper.Utility.Constants;

namespace RS.Dapper.Utility.Connections;
public static class DbIdentifierHelper
{
    /// <summary>
    /// Formats a fully-qualified or simple table name for the current database type.
    /// </summary>
    /// <param name="tableName">
    /// The table name, optionally including a schema (e.g., "dbo.Category", "public.Users", or "Category").
    /// </param>
    /// <returns>
    /// A properly quoted table name based on the configured <c>_dbType</c>:
    /// <list type="bullet">
    /// <item><description>PostgreSQL → "schema"."table" or "table"</description></item>
    /// <item><description>SQL Server → [schema].[table] or [table]</description></item>
    /// <item><description>MySQL → `table` (schema ignored)</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// If the schema is missing, the method will still work and return only the table name.
    /// Any existing quotes/brackets in <paramref name="tableName"/> are stripped before formatting.
    /// </remarks>
    public static string GetTable(string tableName, string schema, DatabaseType dbType)
    {
         
        return dbType switch
        {
            DatabaseType.PostgreSql => $"\"{schema}\".\"{tableName}\"",
            DatabaseType.SqlServer => $"[{schema}].[{tableName}]",
            DatabaseType.MySql => $"`{tableName}`", // MySQL often doesn't use schema names
            _ => tableName
        };
    }
    /// <summary>
    /// Formats a stored procedure name for the current database type.
    /// </summary>
    /// <param name="procedureName">
    /// The procedure name, optionally including a schema (e.g., "dbo.MyProc", "public.calculate_totals", or "MyProc").
    /// </param>
    /// <returns>
    /// A properly quoted procedure name based on the configured <c>_dbType</c>:
    /// <list type="bullet">
    /// <item><description>PostgreSQL → "schema"."procedure" or "procedure"</description></item>
    /// <item><description>SQL Server → [schema].[procedure] or [procedure]</description></item>
    /// <item><description>MySQL → `procedure` (schema ignored)</description></item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// If the schema is missing, the method will still work and return only the procedure name.
    /// Any existing quotes/brackets in <paramref name="procedureName"/> are stripped before formatting.
    /// </remarks>
    public static string GetProcedure(string tableName, string schema, DatabaseType dbType)
    {

        return dbType switch
        {
            DatabaseType.PostgreSql => $"\"{schema}\".\"{tableName}\"",
            DatabaseType.SqlServer => $"[{schema}].[{tableName}]",
            DatabaseType.MySql => $"`{tableName}`", // MySQL often doesn't use schema names
            _ => tableName
        };
    }
}
