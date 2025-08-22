using ProjectName.Models.Master;
using RS.Utilities.BaseResponseModel;
using RS.Utilities.Constants;
using RS.Dapper.Utility.Repositories.DapperRepository;
using ProjectName.DataAccess.Constants;

namespace ProjectName.DataAccess.Master.Category;
public class CategoryRepository(IDapperRepository dapperRepository) :  ICategoryRepository
{
    private readonly IDapperRepository _dapperRepository = dapperRepository;
    public async Task<BaseResponse<int>> CreateAsync(CategoryModel request)
    {
        int id = await _dapperRepository.InsertAsync(request,DbSchema.CategoriesDbName, DbSchema.CategoryTable);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectCreatedSuccess("category"),Data=id };
    }
    public async Task<BaseResponse<int>> UpdateAsync(CategoryModel request)
    {
         await _dapperRepository.UpdateAsync(request, DbSchema.CategoriesDbName, DbSchema.CategoryTable);
        return new BaseResponse<int> { IsSuccess = true, Message = MainMessages.SubjectUpdatedSuccess("category") };
    }
    public async Task<BaseResponse<bool>> DeleteAsync(int id)
    {
        await _dapperRepository.DeleteAsync(id, DbSchema.CategoriesDbName, DbSchema.CategoryTable);
        return new BaseResponse<bool> { IsSuccess = true, Message = MainMessages.SubjectDeletedSuccess("category") };
    }

    public async Task<BaseResponse<IEnumerable<CategoryResponse>>> GetAllAsync()
    {
        return new BaseResponse<IEnumerable<CategoryResponse>>
        {
            Data = await _dapperRepository.GetAllAsync<CategoryResponse>(DbSchema.CategoriesDbName, DbSchema.CategoryTable),
            IsSuccess=true
        };

    }

    public async Task<BaseResponse<CategoryModel>> GetByIdAsync(int id)
    {
        var data=  await _dapperRepository.GetByIdAsync<CategoryModel,int>(id, DbSchema.CategoriesDbName, DbSchema.CategoryTable);
        return BaseResponse<CategoryModel>.FromData(data, null);
    }

    public async Task<BaseResponse<PagedResult<CategoryResponse>>> GetPagedDataAsync(PagedRequest pagedRequest)
    {
        return new BaseResponse<PagedResult<CategoryResponse>>
        {
            Data=await _dapperRepository.GetPagedDataAsync<CategoryResponse>(DbSchema.CategoriesDbName, DbSchema.CategoryTable, pagedRequest),
            IsSuccess=true
        };
    }
}
