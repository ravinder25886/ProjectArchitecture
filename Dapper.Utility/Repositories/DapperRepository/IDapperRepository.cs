namespace RS.Dapper.Utility.Repositories.DapperRepository;
public interface IDapperRepository
{
    public Task<int> InsertAsync<T>(T model, string dbNameKey, string tableName, string keyColumn = "Id");
    public Task<int> UpdateAsync<T>(T model, string dbNameKey, string tableName, string keyColumn = "Id");
    public Task<int> DeleteAsync(object id, string dbNameKey, string tableName, string keyColumn = "Id");
    public Task<TModel?> GetByIdAsync<TModel, TId>(TId id, string dbNameKey, string tableName, string keyColumn = "Id");
    public Task<IEnumerable<T>> GetAllAsync<T>(string dbNameKey, string tableName);
    public Task<PagedResult<T>> GetPagedDataAsync<T>(string dbNameKey, string tableName, PagedRequest pagedRequest);
}
