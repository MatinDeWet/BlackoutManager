using BlackoutManager.DATA.Dtos;
using Microsoft.AspNetCore.Identity;

namespace BlackoutManager.API.SERVICE;

public interface IAuthService
{
    Task<IEnumerable<IdentityError>> Register(UserRegisterDto userRegisterDto);
    Task<UserAuthResponseDto> Login(UserLoginDto userLoginDto);
    Task<UserAuthResponseDto> VerifyRefreshToken(UserAuthResponseDto request);
}
