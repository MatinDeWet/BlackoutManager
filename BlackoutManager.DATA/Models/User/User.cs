using Microsoft.AspNetCore.Identity;

namespace BlackoutManager.DATA.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
