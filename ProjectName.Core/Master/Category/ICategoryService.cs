using ProjectName.Models;
using ProjectName.Models.Master;
using RS.Utilities.BaseResponseModel;

namespace ProjectName.Core.Master.Category;
public interface ICategoryService
{
    public Task<BaseResponse<int>> AddAsync(CategoryModel request);
    public Task<BaseResponse<int>> UpdateAsync(CategoryModel request);
    public Task<BaseResponse<bool>> DeleteAsync(int id);
    public Task<BaseResponse<CategoryModel>> GetByIdAsync(int id);
    public Task<BaseResponse<IEnumerable<CategoryResponse>>> GetAllAsync();
    public Task<BaseResponse<PagedResult<CategoryResponse>>> GetPagedDataAsync(SearchRequest searchRequest);
}
