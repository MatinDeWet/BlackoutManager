using System.ComponentModel.DataAnnotations;

namespace BlackoutManager.DATA.Dtos;

public class UserRegisterDto : UserLoginDto
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;
}
