using Blog.Domain.Entities;
using Blog.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[ServiceFilter(typeof(ValidateUserFilter))] // ⬅️ Đăng ký filter ở đây
[Route("api/[controller]")]
public abstract class AuthorizeController : ControllerBase
{
    public User? CurrentUser;
    private IUserService userService;

    protected AuthorizeController(IUserService userService)
    {
        this.userService = userService;
    }
}
