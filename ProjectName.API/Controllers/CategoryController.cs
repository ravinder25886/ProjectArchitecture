using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProjectName.Core.Master.Category;
using ProjectName.Models.Master;
using ProjectName.Utilities.BaseResponseModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectName.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    public readonly ICategoryService _categoryService  = categoryService;

    // GET: api/<CategoryController>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        
        BaseResponse <IEnumerable<CategoryModel>> response= await _categoryService.GetAllAsync();
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    // GET api/<CategoryController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        BaseResponse<CategoryModel > response= await _categoryService.GetByIdAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);

    }

    // POST api/<CategoryController>
    [HttpPost]
    public async Task<IActionResult> Post(CategoryRequest request)
    {
        BaseResponse<int> response = await _categoryService.AddAsync(new CategoryModel
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive
        });
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    // PUT api/<CategoryController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, CategoryRequest request)
    {
        BaseResponse<int> response = await _categoryService.UpdateAsync(new CategoryModel
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            IsActive = request.IsActive
        });
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    // DELETE api/<CategoryController>/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        BaseResponse<bool> response =await _categoryService.DeleteAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }
}
public class CategoryRequest 
{
    [MaxLength(255)]
    public required string Name { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

