using Blog.Domain;
using Blog.Domain.Entities;
using Blog.Domain.Interface;
using Blog.Domain.ViewModel.User;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : AuthorizeController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger) : base(userService)
        {
            _userService = userService;
            _logger = logger;
        }

        // POST: api/user/create
        [HttpPost]
        [Route("create")]
        public async Task<ResponseResult> CreateUser([FromBody] CreateUserViewModel model)
        {
            var result = await _userService.Create(model);
            return result;
        }

        // GET: api/user/get-all
        [HttpGet]
        [Route("get-all")]
        public async Task<ResponseResult> GetAllUser()
        {
            try
            {
                var result = await _userService.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                // log nếu cần: _logger.LogError(ex, "Error when get all users");
                throw new Exception(ex.Message);
            }
        }

        // POST: api/user/update
        [HttpPost]
        [Route("update")]
        public async Task<ResponseResult> UpdateUser([FromBody] UpdateUserViewModel model)
        {
            var result = await _userService.Update(model);
            return result;
        }

        // GET: api/user/get-by-id/{id}
        [HttpGet("get-by-id/{id}")]
        public async Task<ResponseResult<UserViewModel>> GetUserById(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new Exception("Id is required");
            }

            var result = await _userService.GetById(id);
            return result;
        }

        // DELETE: api/user/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<ResponseResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new Exception("Id is required");
            }

            var result = await _userService.Delete(id);
            return result;
        }

    }
}
