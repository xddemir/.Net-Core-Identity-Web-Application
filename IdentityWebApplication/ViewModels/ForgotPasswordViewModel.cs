using System.ComponentModel.DataAnnotations;

namespace IdentityWebApplication.Models;

public class ForgotPasswordViewModel
{
    [EmailAddress(ErrorMessage = "Email format incorrect!")]
    [Required(ErrorMessage = "Email Required!")]
    [Display(Name="E-Mail")]
    public string Email { get; set; }
}