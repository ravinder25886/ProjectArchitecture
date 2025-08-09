using ProjectName.DataAccess.Master.Category;
using ProjectName.Models;
using ProjectName.Models.Master;
using RS.Utilities.BaseResponseModel;

namespace ProjectName.Core.Master.Category;
public class CategoryService(ICategoryRepository categoryRepository):ICategoryService
{
    private readonly ICategoryRepository _categoryRepository=categoryRepository;
    public async Task<BaseResponse<int>> AddAsync(CategoryModel request)
    { 
        return await _categoryRepository.CreateAsync(request);
    }
    public async Task<BaseResponse<int>> UpdateAsync(CategoryModel request)
    { 
       return await _categoryRepository.UpdateAsync(request);
    }
    public async Task<BaseResponse<bool>> DeleteAsync(int id)
    {
      return await _categoryRepository.DeleteAsync(id);
    }

    public async Task<BaseResponse<IEnumerable<CategoryResponse>>> GetAllAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<BaseResponse<CategoryModel>> GetByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public Task<BaseResponse<PagedResult<CategoryResponse>>> GetPagedDataAsync(SearchRequest searchRequest)
    {
        List<SqlFilter> filters = new List<SqlFilter>();
       
        if (!string.IsNullOrWhiteSpace(searchRequest.SeachText))
        {
            filters.Add(new SqlFilter("Name", SqlOperator.Like, $"%{searchRequest.SeachText}%"));
        }
        PagedRequest pagedRequest = new PagedRequest
        {
            PageNumber=searchRequest.PageNumber,
            PageSize = searchRequest.PageSize,
            OrderBy =searchRequest.OrderBy,
            SortDirection=searchRequest.SortDirection,
            Filters= filters 
        };
        return _categoryRepository.GetPagedDataAsync(pagedRequest);
    }
}
