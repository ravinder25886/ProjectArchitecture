namespace RS.Dapper.Utility;
public interface IDapperRepository
{
    public Task<int> InsertAsync<T>(T model, string tableName, string keyColumn = "Id");
    public Task<int> UpdateAsync<T>(T model, string tableName, string keyColumn = "Id");
    public Task<int> DeleteAsync(object id, string tableName, string keyColumn = "Id");
    public Task<TModel?> GetByIdAsync<TModel, TId>(TId id, string tableName, string keyColumn = "Id");
    public Task<IEnumerable<T>> GetAllAsync<T>(string tableName);
    public Task<PagedResult<T>> GetPagedDataAsync<T>(string tableName, PagedRequest pagedRequest);
}
