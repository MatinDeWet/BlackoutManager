using System.ComponentModel.DataAnnotations;

namespace BlackoutManager.DATA.Dtos;

public class UserLoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(15, ErrorMessage = "Your Password is limited to {2} to {1} characters", MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
