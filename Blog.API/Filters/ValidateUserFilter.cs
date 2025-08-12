using Blog.Domain.Entities;
using Blog.Domain.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class ValidateUserFilter : IAsyncActionFilter
{
    private readonly IUserService _userService;

    public ValidateUserFilter(IUserService userService)
    {
        _userService = userService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userIdStr = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdStr, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var user = await _userService.FindUserById(userId);
        if (user == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Gán user cho Controller nếu cần
        if (context.Controller is AuthorizeController controller)
        {
            controller.CurrentUser = user;
        }

        await next();
    }
}
