public class SqlFilter(string column, string op, object value)
{
    public string Column { get; set; } = column;
    public string Operator { get; set; }=op;
    public object Value { get; set; } = value;
}
