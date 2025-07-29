using ProjectName.Models.Master;
using ProjectName.Utilities.BaseResponseModel;

namespace ProjectName.DataAccess.Master.Category;
public interface ICategoryRepository
{
    public Task<BaseResponse<int>> CreateAsync(CategoryModel user);
    public Task<BaseResponse<int>> UpdateAsync(CategoryModel user);
    public Task<BaseResponse<int>> DeleteAsync(int id);
    public Task<BaseResponse<CategoryModel>> GetByIdAsync(int id);
    public Task<BaseResponse<IEnumerable<CategoryModel>>> GetAllAsync();
}
