using System.ComponentModel.DataAnnotations;

namespace IdentityWebApplication.ViewModels;

public class PasswordChangeViewModel
{ 
    [Required(ErrorMessage="Old Password Required!")]
    [Display(Name="New Password")]
    public string PasswordOld { get; set; }
    [Required(ErrorMessage="New Password Required!")]
    [Display(Name="New Password")]
    public string PasswordNew { get; set; }
    [Required(ErrorMessage="Password Required!")]
    [Display(Name="New Password")]
    [Compare(nameof(PasswordNew), ErrorMessage = "Passwords not matching")]
    public string PasswordConfirm { get; set; }
    
}