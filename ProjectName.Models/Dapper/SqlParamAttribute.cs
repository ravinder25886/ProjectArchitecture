namespace ProjectName.Models.Dapper;
// This attribute allows you to specify the name of the SQL parameter
// If omitted, it will use the property name
[AttributeUsage(AttributeTargets.Property)]
public class SqlParamAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
