using RS.Dapper.Utility.Constants;

public static class SqlOperatorExtensions
{
    public static string ToSqlString(this SqlOperator op, DatabaseType dbType)
    {
        return op switch
        {
            SqlOperator.Equal => "=",
            SqlOperator.NotEqual => "<>",
            SqlOperator.GreaterThan => ">",
            SqlOperator.GreaterThanOrEqual => ">=",
            SqlOperator.LessThan => "<",
            SqlOperator.LessThanOrEqual => "<=",

            // LIKE: PostgreSQL uses ILIKE for case-insensitive match
            SqlOperator.Like => dbType == DatabaseType.PostgreSql ? "ILIKE" : "LIKE",
            SqlOperator.NotLike => dbType == DatabaseType.PostgreSql ? "NOT ILIKE" : "NOT LIKE",

            SqlOperator.In => "IN",
            SqlOperator.NotIn => "NOT IN",
            SqlOperator.Between => "BETWEEN",

            SqlOperator.IsNull => "IS NULL",
            SqlOperator.IsNotNull => "IS NOT NULL",

            _ => throw new NotSupportedException($"Unsupported SQL operator: {op}")
        };
    }
}
