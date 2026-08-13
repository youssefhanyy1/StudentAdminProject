using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace StudentAdminProject.Authorization
{
    public class SameUserOrAdminRequirement : IAuthorizationRequirement { }

    public class SameUserOrAdminHandler : AuthorizationHandler<SameUserOrAdminRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SameUserOrAdminHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return Task.CompletedTask;

            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var loggedInUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var routeUserId = httpContext.Request.RouteValues["userId"]?.ToString();

            // 4. التحقق مما إذا كان الطالب يطلب بياناته الشخصية بالفعل
            if (!string.IsNullOrEmpty(loggedInUserId) && loggedInUserId == routeUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}