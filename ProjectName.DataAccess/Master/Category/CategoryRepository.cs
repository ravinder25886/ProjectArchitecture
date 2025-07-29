using ProjectName.DataAccess.Constants;
using ProjectName.Models.Account;
using ProjectName.Models.Master;
using ProjectName.Utilities.BaseResponseModel;
using ProjectName.Utilities.Constants;

using RS.Dapper.Utility;

namespace ProjectName.DataAccess.Master.Category;
public class CategoryRepository(IDapperRepository dapperRepository) :  ICategoryRepository
{
    private readonly IDapperRepository _dapperRepository = dapperRepository;
    public async Task<BaseResponse<int>> CreateAsync(CategoryModel user)
    {
        int id = await _dapperRepository.InsertAsync(user, DbSchema.CategoryTable);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectCreatedSuccess("category") };
    }
    public async Task<BaseResponse<int>> UpdateAsync(CategoryModel user)
    {
        int id = await _dapperRepository.UpdateAsync(user, DbSchema.CategoryTable);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectUpdatedSuccess("category") };
    }
    public async Task<BaseResponse<int>> DeleteAsync(int id)
    {
        await _dapperRepository.DeleteAsync(id, DbSchema.CategoryTable);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectDeletedSuccess("category") };
    }

    public async Task<BaseResponse<IEnumerable<CategoryModel>>> GetAllAsync()
    {
        return new BaseResponse<IEnumerable<CategoryModel>>
        {
            Data = await _dapperRepository.GetAllAsync<CategoryModel>(DbSchema.CategoryTable)
        };
    }

    public async Task<BaseResponse<CategoryModel>> GetByIdAsync(int id)
    {
        var data=  await _dapperRepository.GetByIdAsync<CategoryModel,int>(id, DbSchema.CategoryTable);
        return BaseResponse<CategoryModel>.FromData(data, null);
    }
}
