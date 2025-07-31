using ProjectName.Models.Master;
using ProjectName.Utilities.BaseResponseModel;

namespace ProjectName.DataAccess.Master.Category;
public interface ICategoryRepository
{
    public Task<BaseResponse<int>> CreateAsync(CategoryModel request);
    public Task<BaseResponse<int>> UpdateAsync(CategoryModel request);
    public Task<BaseResponse<bool>> DeleteAsync(int id);
    public Task<BaseResponse<CategoryModel>> GetByIdAsync(int id);
    public Task<BaseResponse<IEnumerable<CategoryResponse>>> GetAllAsync();
    public Task<BaseResponse<PagedResult<CategoryResponse>>> GetPagedDataAsync(PagedRequest pagedRequest);
}
