using System;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var userContext = resultContext.HttpContext.User;

            if (!userContext.Identity.IsAuthenticated) return;

            var userId = userContext.GetUserId();
            var uow = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await uow.UserRepository .GetUserByIdAsync(userId);
            
            user.LastActive = DateTime.UtcNow;

            await uow.Complete();
        }
    }
}