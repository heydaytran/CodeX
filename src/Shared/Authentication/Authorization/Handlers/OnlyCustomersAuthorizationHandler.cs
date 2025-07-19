namespace Authentication.Authorization.Handlers;

public class OnlyCustomersAuthorizationHandler : AuthorizationHandler<OnlyCustomersRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OnlyCustomersRequirement requirement
    )
    {
        if (context.User.IsInRole(Roles.Customer))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}