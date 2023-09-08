using IdentityWebApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityWebApplication.CustomValidations;

public class UserValidator : IUserValidator<AppUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        var errors = new List<IdentityError>();

        var isNumeric = int.TryParse(user.UserName[0].ToString(), out _);

        if (isNumeric)
        {
            errors.Add(new()
            {
                Code = "UserNameContainFirstLetterDigit",
                Description = "First digit must not be numeric"
            });
        }

        if (errors.Any())
        {
            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }

        return Task.FromResult(IdentityResult.Success);

    }
}