namespace BlackoutManager.DATA.Dtos;

public class UserAuthResponseDto
{
    public string UserID { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
