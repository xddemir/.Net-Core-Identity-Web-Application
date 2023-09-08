using System.ComponentModel.DataAnnotations;

namespace IdentityWebApplication.ViewModels;

public class SignUpViewModel
{
    [Display(Name="User Name")]
    [Required(ErrorMessage = "UserName Required!")]
    public string UserName { get; set; }
    
    [EmailAddress(ErrorMessage = "Email format incorrect!")]
    [Required(ErrorMessage = "Email Required!")]
    [Display(Name="E-Mail")]
    public string EmailAddress { get; set; }
    
    [Required(ErrorMessage = "Phone Required!")]
    [Display(Name="Phone")]
    public string Phone { get; set; }
    
    [Required(ErrorMessage = "Password Required!")]
    [Display(Name="Password")]
    public string Password { get; set; }
    
    [Required(ErrorMessage="Password Required!")]
    [Display(Name="Password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords not matching")]
    public string PasswordConfirm { get; set; }
}