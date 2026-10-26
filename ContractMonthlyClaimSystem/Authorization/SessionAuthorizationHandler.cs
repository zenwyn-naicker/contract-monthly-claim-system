/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Policy-based authorization in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0> [Accessed 16 November 2025]
 **************************************************************************************************************/

using Microsoft.AspNetCore.Authorization;

namespace ContractMonthlyClaimSystem.Authorization
{
    //_______________________________ In-Text Referencing _____________________________//
    //The following layout was taken from Microsoft (2025) Policy-based authorization in ASP.NET Core

    // Requirement for role-based authorization
    public class RoleRequirement : IAuthorizationRequirement
    {
        public string RequiredRole { get; }

        public RoleRequirement(string role)
        {
            RequiredRole = role;
        }
    }

    // Handler that checks session for user role
    public class SessionAuthorizationHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RoleRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return Task.CompletedTask;
            }

            // Check if user is authenticated via session
            var userId = httpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Task.CompletedTask; // Not authenticated
            }

            // Check if user has the required role
            var userRole = httpContext.Session.GetString("UserRole");
            if (userRole == requirement.RequiredRole)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }

    //_______________________________________ End _____________________________________//
}