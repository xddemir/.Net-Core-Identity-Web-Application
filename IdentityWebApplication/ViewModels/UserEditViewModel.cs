using System.ComponentModel.DataAnnotations;
using IdentityWebApplication.Models;

namespace IdentityWebApplication.ViewModels;

public class UserEditViewModel
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
    
    [Required(ErrorMessage = "Picture Required!")]
    [Display(Name="Picture")]
    public IFormFile? Picture { get; set; }
    
    [DataType(DataType.Date)]
    [Required(ErrorMessage = "BirthDate Required!")]
    [Display(Name="BirthDate")]
    public DateTime? BirthDate { get; set; }
    
    [Required(ErrorMessage = "City Required!")]
    [Display(Name="City")]
    public string? City { get; set; }
    
    [Required(ErrorMessage = "Gender Required!")]
    [Display(Name="Gender")]
    public Gender? Gender { get; set; }
   
    
   
}