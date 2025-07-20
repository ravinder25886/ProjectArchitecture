namespace ProjectName.DataAccess.Constants;
public static class DbUser
{
    private const string Schema = "user.";

    // Tables
    public static string UserTable => $"{Schema}User";

    // Stored Procedures
    public static string GetUserByIdProc => $"{Schema}GetUserById";
    public static string InsertUserProc => $"{Schema}InsertUser";
    public static string UpdateUserProc => $"{Schema}UpdateUser";
}
