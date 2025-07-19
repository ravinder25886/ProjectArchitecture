using System.Data;
using Dapper;

using ProjectName.Infrastructure.Connections;

public abstract class BaseRepository
{
    private readonly DapperContext _context;

    protected BaseRepository(DapperContext context)
    {
        _context = context;
    }

    // Reusable connection property
    protected IDbConnection Database => _context.CreateConnection();
}
