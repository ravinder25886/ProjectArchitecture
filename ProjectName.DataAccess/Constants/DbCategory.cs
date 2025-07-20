namespace ProjectName.DataAccess.Constants;
public static class DbCategory
{
    private const string Schema = "category.";

    // Tables
    public static string CategoryTable => $"{Schema}Category";

    // Stored Procedures
    public static string GetCategoryByIdProc => $"{Schema}GetCategoryById";
    public static string InsertCategoryProc => $"{Schema}InsertCategory";
    public static string DeleteCategoryProc => $"{Schema}DeleteCategory";
}
