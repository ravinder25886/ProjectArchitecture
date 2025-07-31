namespace RS.Dapper.Utility.Attributes;
// This attribute is used to ignore a property during parameter generation
[AttributeUsage(AttributeTargets.Property)]
public class IgnoreParamAttribute : Attribute
{
}
[AttributeUsage(AttributeTargets.Property)]
public class IgnoreOnInsertAttribute : Attribute { }
