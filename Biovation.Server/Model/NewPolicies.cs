// A handler that can determine whether a MaximumOfficeNumberRequirement is satisfied
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

internal class OverrideTestAuthorizationHandler : AuthorizationHandler<OverrideTestRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OverrideTestRequirement requirement)
    {
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}

// A custom authorization requirement which requires office number to be below a certain value
internal class OverrideTestRequirement : IAuthorizationRequirement
{
    public OverrideTestRequirement(int officeNumber)
    {
        OverrideTestNumber = 0;
    }

    public int OverrideTestNumber { get; private set; }
}