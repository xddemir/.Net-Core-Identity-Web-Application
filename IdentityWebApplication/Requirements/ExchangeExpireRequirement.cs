using Microsoft.AspNetCore.Authorization;

namespace IdentityWebApplication.Requirements;

public class ExchangeExpireRequirement : IAuthorizationRequirement
{
    
    
}

public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
    {
        var hasExchangeExpireClaim = context.User.HasClaim(x=> x.Type == "ExchangeExpireDate");

        if (!hasExchangeExpireClaim) {
             context.Fail();
        }

        var exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate");

        if (Convert.ToDateTime(exchangeExpireDate.Value) < DateTime.Now) {
            context.Fail();
        }
        else {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;

    }
}