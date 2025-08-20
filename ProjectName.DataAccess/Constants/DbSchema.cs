using Microsoft.Extensions.Configuration;

using RS.Dapper.Utility.Connections;
using RS.Dapper.Utility.Constants;
using RS.Dapper.Utility.Models;

namespace ProjectName.DataAccess.Constants;
public static class DbSchema
{
    private static Dictionary<string, DatabaseConfig>? _configs;

    public const string UsersDbNameKey = "UsersDb";
    private const string _userDbSchema = "dbo";

    public const string CategoriesDbNameKey = "CategoriesDb";
    private const string _categoriesDbSchema = "";

    private static DatabaseType _userDBType = DatabaseType.SqlServer;
    private static DatabaseType _categoriesDBType = DatabaseType.SqlServer;
    public static void Initialize(IConfiguration configuration)
    {


        _configs = configuration.GetSection("Databases")
                               .GetChildren()
                               .ToDictionary(
                                   x => x.Key,
                                   static x => new DatabaseConfig
                                   {
                                       DbType = Enum.Parse<DatabaseType>(value: x["DbType"] ?? "SqlServer")
                                   });

        //var dbSettings = configuration.GetSection("DatabaseSettings");
        //Enum.TryParse(dbSettings["Type"], out _dbType);

        _userDBType = GetDatabaseType("UsersDb");       // returns DatabaseType.SqlServer
        _categoriesDBType = GetDatabaseType("CategoriesDb"); // returns DatabaseType.MySql
        //var productsDbType = GetDatabaseType("ProductsDb"); // returns DatabaseType.PostgreSql
    }
    private static DatabaseType GetDatabaseType(string dbName)
    {
        var config = _configs?.GetValueOrDefault(dbName)
                     ?? throw new KeyNotFoundException($"Database configuration for '{dbName}' not found.");

        return config.DbType;
    }
    // Tables
    public static string UserTable => DbIdentifierHelper.GetTable($"User", _userDbSchema, _userDBType);
    public static string CategoryTable => DbIdentifierHelper.GetTable($"Category", _categoriesDbSchema, _categoriesDBType);

    // Stored Procedures
    public static string UserGetByIdProc => DbIdentifierHelper.GetProcedure($"User_GetById", _userDbSchema, _userDBType);
    public static string UserInsertProc => DbIdentifierHelper.GetProcedure($"User_Insert", _userDbSchema, _userDBType);
    public static string UserUpdateProc => DbIdentifierHelper.GetProcedure($"User_Update", _userDbSchema, _userDBType);
    public static string UserDeleteProc => DbIdentifierHelper.GetProcedure($"User_Delete", _userDbSchema, _userDBType);
    public static string UserGetAllProc => DbIdentifierHelper.GetProcedure($"User_GetAll", _userDbSchema, _userDBType);
    
}
