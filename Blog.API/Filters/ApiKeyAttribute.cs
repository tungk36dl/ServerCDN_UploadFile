using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Blog.API.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private const string API_KEY_HEADER_NAME = "X-API-KEY";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var config = context.HttpContext.RequestServices.GetService<IConfiguration>();
            var expectedApiKey = config.GetValue<string>("ApiKeySettings:Key");

            if (!context.HttpContext.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("API Key is missing.");
                return;
            }

            if (!expectedApiKey.Equals(extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Invalid API Key.");
                return;
            }

            await next();
        }
    }
}

