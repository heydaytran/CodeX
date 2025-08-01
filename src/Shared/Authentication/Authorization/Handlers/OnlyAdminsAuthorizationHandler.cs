namespace Authentication.Authorization.Handlers;

public class OnlyAdminsAuthorizationHandler : AuthorizationHandler<OnlyAdminsRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OnlyAdminsRequirement requirement
    )
    {
        if (context.User.IsInRole(Roles.Admin))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}