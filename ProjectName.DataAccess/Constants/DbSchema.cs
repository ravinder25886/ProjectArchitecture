using Microsoft.Extensions.Configuration;

using RS.Dapper.Utility.Connections;
using RS.Dapper.Utility.Constants;

namespace ProjectName.DataAccess.Constants;
public static class DbSchema
{
    //private const string Schema = "dbo";
    private const string Schema = "";// In MYSQL database case we canot use any Schema
    //public const string Schema = "public";// In PostgreSql database case we canot use any Schema

    private static DatabaseType _dbType = DatabaseType.SqlServer;
    public static void Initialize(IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("DatabaseSettings");
        Enum.TryParse(dbSettings["Type"], out _dbType);
    }
    // Tables
    public static string UserTable => DbIdentifierHelper.GetTable($"User", Schema, _dbType);
    public static string CategoryTable => DbIdentifierHelper.GetTable($"Category", Schema, _dbType);

    // Stored Procedures
    public static string UserGetByIdProc => DbIdentifierHelper.GetProcedure($"User_GetById", Schema, _dbType);
    public static string UserInsertProc => DbIdentifierHelper.GetProcedure($"User_Insert", Schema, _dbType);
    public static string UserUpdateProc => DbIdentifierHelper.GetProcedure($"User_Update", Schema, _dbType);
    public static string UserDeleteProc => DbIdentifierHelper.GetProcedure($"User_Delete", Schema, _dbType);
    public static string UserGetAllProc => DbIdentifierHelper.GetProcedure($"User_GetAll", Schema, _dbType);
    
}
