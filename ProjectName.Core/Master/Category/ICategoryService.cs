using ProjectName.Models.Master;
using ProjectName.Utilities.BaseResponseModel;

namespace ProjectName.Core.Master.Category;
public interface ICategoryService
{
    public Task<BaseResponse<int>> AddAsync(CategoryModel request);
    public Task<BaseResponse<int>> UpdateAsync(CategoryModel request);
    public Task<BaseResponse<int>> DeleteAsync(int id);
    public Task<BaseResponse<CategoryModel>> GetByIdAsync(int id);
    public Task<BaseResponse<IEnumerable<CategoryModel>>> GetAllAsync();
}
