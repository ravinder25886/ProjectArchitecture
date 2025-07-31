public class SqlFilter(string column, SqlOperator op, object value)
{
    public string Column { get; set; } = column;
    public SqlOperator Operator { get; set; } = op;
    public object Value { get; set; } = value;
}
