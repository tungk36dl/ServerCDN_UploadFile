using Blog.Domain;
using Blog.Domain.Interface;
using Blog.Domain.ViewModel.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<ResponseResult> Login([FromBody] LoginUserViewModel model)
        {
            try
            {
                var result = await _userService.LoginAsync(model);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                throw;
            }
        }
        // POST: api/auth/logout
        [Authorize]                        // bắt buộc có token
        [HttpPost("logout")]
        public async Task<ResponseResult> Logout()
        {
            // Lấy access-token hiện tại từ header
            var raw = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var token = raw?.Replace("Bearer ", string.Empty);

            if (string.IsNullOrWhiteSpace(token))
                return ResponseResult.Fail("Token not found");

            // ---- Cách 1: chỉ client xoá token (không blacklist) ----
            // Trả về 200 OK, client tự xoá localStorage / cookie
            // return ResponseResult.Success("Logged out");


            // ---- Cách 2: Ghi token vào Blacklist ----
            //if (_blacklist != null)
            //    await _blacklist.RevokeAsync(token);

            return ResponseResult.Success("Logged out & token revoked");
        }
    }
}
