public static class SqlFilterHelper
{
    public static bool IsValidFilterValue(object value)
    {
        if (value == null || value == DBNull.Value)
        {
            return false;
        }

        if (value is string str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        // Optional: Skip DateTime.MinValue, 0 for int, etc. if needed
        if (value is DateTime dt)
        {
            return dt != DateTime.MinValue;
        }

        if (value is int i)
        {
            return i != 0;
        }

        return true; // For all other types (bool, decimal, enums, etc.)
    }
}
