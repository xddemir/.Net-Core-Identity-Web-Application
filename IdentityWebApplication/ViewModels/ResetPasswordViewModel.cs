using System.ComponentModel.DataAnnotations;

namespace IdentityWebApplication.ViewModels;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Password Required!")]
    [Display(Name="New Password")]
    public string Password { get; set; }
    
    [Required(ErrorMessage="Password Required!")]
    [Display(Name="New Password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords not matching")]
    public string PasswordConfirm { get; set; }
}