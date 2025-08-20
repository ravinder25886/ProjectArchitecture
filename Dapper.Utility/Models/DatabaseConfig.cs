using RS.Dapper.Utility.Constants;

namespace RS.Dapper.Utility.Models;
public class DatabaseConfig
{
    public DatabaseType DbType { get; set; }
    public string Schema { get; set; } = string.Empty;
    public string? ConnectionString { get; set; }
}
