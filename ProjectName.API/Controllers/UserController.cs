using System.Threading.Tasks;

using Azure;

using Microsoft.AspNetCore.Mvc;

using ProjectName.Core.User;
using ProjectName.Models.Account;
using ProjectName.Models.Master;
using ProjectName.Utilities.BaseResponseModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectName.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    // GET: api/<UserController>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        BaseResponse<IEnumerable<UserModel>> response = await _userService.GetAllAsync();
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    // GET api/<UserController>/5
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        BaseResponse<UserModel> response = await _userService.GetByIdAsync(id);
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    // POST api/<UserController>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserRequest createUserRequest)
    {
        BaseResponse<int> response= await _userService.AddAsync(new Models.Account.UserModel
        {
            Email= createUserRequest.Email,
            FullName= createUserRequest.FullName,
            Password= createUserRequest.Password
        });
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    // PUT api/<UserController>/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, UserRequest createUserRequest)
    {
        BaseResponse<int> response = await _userService.UpdateAsync(new Models.Account.UserModel
        {
            Id=id,
            Email = createUserRequest.Email,
            FullName = createUserRequest.FullName,
            Password = createUserRequest.Password
        });
        return response.IsSuccess ? Ok(response) : BadRequest(response);
    }

    // DELETE api/<UserController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}
public class UserRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
