using System.Reflection;

using Dapper;

namespace ProjectName.Domain.Dapper;
// This abstract base class helps convert any derived model into Dapper DynamicParameters
using System.Reflection;

using Dapper;

public abstract class DapperModel
{
    // Convert all properties except those marked [IgnoreParam]
    // Use this for Update where Id is required
    public DynamicParameters ToParametersForUpdate()
    {
        var parameters = new DynamicParameters();
        var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            if (prop.GetCustomAttribute<IgnoreParamAttribute>() != null)
                continue;

            var attr = prop.GetCustomAttribute<SqlParamAttribute>();
            var paramName = attr?.Name ?? prop.Name;
            var value = prop.GetValue(this);

            parameters.Add("@" + paramName, value);
        }
        return parameters;
    }

    // Convert all properties except [IgnoreParam] and "Id"
    // Use this for Insert where Id should NOT be passed
    public DynamicParameters ToParametersForInsert()
    {
        var parameters = new DynamicParameters();
        var props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            if (prop.GetCustomAttribute<IgnoreParamAttribute>() != null)
                continue;

            if (string.Equals(prop.Name, "Id", StringComparison.OrdinalIgnoreCase))
                continue; // Skip Id for insert

            var attr = prop.GetCustomAttribute<SqlParamAttribute>();
            var paramName = attr?.Name ?? prop.Name;
            var value = prop.GetValue(this);

            parameters.Add("@" + paramName, value);
        }
        return parameters;
    }
}
