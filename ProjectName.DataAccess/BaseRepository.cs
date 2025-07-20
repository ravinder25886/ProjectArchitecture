using System.Data;
using Dapper;

using ProjectName.DataAccess.Connections;

public abstract class BaseRepository(DapperContext context)
{
    private readonly DapperContext _context = context;

    // Reusable connection property
    protected IDbConnection Database => _context.CreateConnection();
}
