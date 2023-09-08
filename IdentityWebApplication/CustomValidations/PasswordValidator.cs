using IdentityWebApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityWebApplication.CustomValidations;

public class PasswordValidator : IPasswordValidator<AppUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
    {
        var errors = new List<IdentityError>();
        
        if (password!.ToLower().Contains(user.UserName!.ToLower())) 
        {
            errors.Add(new()
            {
                Code = "PasswordContainUserName",
                Description = "Password must not include user name"
            });
        }

        if (password!.ToLower().StartsWith("1234"))
        {
            errors.Add(new()
            {
                Code = "PasswordContain1234",
                Description = "Password must not start with (1234)"
            });
        }

        if (!errors.Any()) {
            return Task.FromResult(IdentityResult.Success);
        }

        return Task.FromResult(IdentityResult.Failed(errors.ToArray()));

    }
}