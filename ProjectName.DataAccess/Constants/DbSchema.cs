namespace ProjectName.DataAccess.Constants;
public static class DbSchema
{
    //private const string Schema = "dbo.";
    private const string Schema = "";// In MYSQL database case we canot use any Schema
    // Tables
    public static string UserTable => $"{Schema}User";
    public static string CategoryTable => $"{Schema}Category";

    // Stored Procedures
    public static string UserGetByIdProc => $"{Schema}User_GetById";
    public static string UserInsertProc => $"{Schema}User_Insert";
    public static string UserUpdateProc => $"{Schema}User_Update";
}
