using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IdentityWebApplication.Extensions;

public static class ModelStateExtension
{
    public static void AddModelErrorList(this ModelStateDictionary modelState, List<string> errors)
    {
        foreach (var item in errors)
        {
            modelState.AddModelError(string.Empty, item);
        }
    }
    
    public static void AddModelErrorList(this ModelStateDictionary modelState, IEnumerable<IdentityError> errors)
    {
        foreach (var item in errors.ToList())
        {
            modelState.AddModelError(string.Empty, item.Description);
        }
    }
}