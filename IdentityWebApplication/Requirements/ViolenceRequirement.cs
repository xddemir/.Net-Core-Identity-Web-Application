using Microsoft.AspNetCore.Authorization;

namespace IdentityWebApplication.Requirements;

public class ViolenceRequirement : IAuthorizationRequirement
{
    public int ThresholdAge { get; set; }
    
}

public class ViolenceRequirementHandler : AuthorizationHandler<ViolenceRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ViolenceRequirement requirement)
    {
        var hasExchangeExpireClaim = context.User.HasClaim(x=> x.Type == "Birthdate");

        if (!hasExchangeExpireClaim) {
            context.Fail();
        }

        var birthDateClaim = context.User.FindFirst("Birthdate");

        var today = DateTime.Now;
        var birthDate = Convert.ToDateTime(birthDateClaim.Value);
        var age = today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age)) age--;
      
        if (requirement.ThresholdAge > age) {
            context.Fail();
        }
        else {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
