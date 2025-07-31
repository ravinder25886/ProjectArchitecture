using ProjectName.Models.Master;
using ProjectName.Utilities.BaseResponseModel;

namespace ProjectName.DataAccess.Master.Category;
public interface ICategoryRepository
{
    public Task<BaseResponse<int>> CreateAsync(CategoryModel user);
    public Task<BaseResponse<int>> UpdateAsync(CategoryModel user);
    public Task<BaseResponse<bool>> DeleteAsync(int id);
    public Task<BaseResponse<CategoryModel>> GetByIdAsync(int id);
    public Task<BaseResponse<IEnumerable<CategoryModel>>> GetAllAsync();
    public Task<BaseResponse<PagedResult<CategoryModel>>> GetPagedDataAsync(PagedRequest pagedRequest);
}
