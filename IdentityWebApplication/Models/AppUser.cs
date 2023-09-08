using Microsoft.AspNetCore.Identity;

namespace IdentityWebApplication.Models;

public class AppUser : IdentityUser
{
    public string? City { get; set; }
    public string? Picture  { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender? Gender { get; set; }
    
}