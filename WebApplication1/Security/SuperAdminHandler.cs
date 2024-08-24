using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Security
{
    public class SuperAdminHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            if(context.User.IsInRole("Admin rady"))
            {
                context.Succeed(requirement);
            }
                return Task.CompletedTask;
        }
    }
}
