using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace WebApplication1.Security
{
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {

            var authFilterContext = context.Resource as AuthorizationFilterContext;

            if (authFilterContext == null)
            {
                return Task.CompletedTask;
            }
            string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string adminIdBeingEdited = authFilterContext.HttpContext.Request.Query["userId"];
            Console.WriteLine(loggedInAdminId);
            Console.WriteLine(adminIdBeingEdited);
            foreach(var claim in context.User.Claims)
            {
                Console.WriteLine($"Claim Type is {claim.Type} and value is {claim.Value}");
            }

            if (context.User.IsInRole("Admin")
                 && context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value=="true" && adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower()))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
