using System.ComponentModel.DataAnnotations;

namespace IdentityWebApplication.ViewModels;

public class SignInViewModel
{
    [EmailAddress(ErrorMessage = "Email format incorrect!")]
    [Required(ErrorMessage = "Email Required!")]
    [Display(Name="Email")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password Required!")]
    [Display(Name="Password")]
    public string? Password { get; set; }

    public bool RememberMe { get; set; }
}