using AutoMapper;
using BlackoutManager.DATA.Dtos;
using BlackoutManager.DATA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlackoutManager.API.SERVICE.Services;

public class AuthService : IAuthService
{
    #region Dependency Injection / CTOR

    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;
    private User _user;
    private const string _loginProvider = "RPGManagerAPI";
    private const string _refreshToken = "RefreshToken";

    public AuthService(IMapper mapper, UserManager<User> userManager, IConfiguration config, ILogger<AuthService> logger)
    {
        _mapper = mapper;
        _userManager = userManager;
        _config = config;
        _logger = logger;
    }
    #endregion

    #region Public Methods
    public async Task<UserAuthResponseDto> Login(UserLoginDto userLoginDto)
    {
        _logger.LogInformation("Login Attempt with email: {email}", userLoginDto.Email);

        _user = await _userManager.FindByEmailAsync(userLoginDto.Email);
        if (_user is null)
        {
            _logger.LogWarning("Login Failed: could not find user with email ({email})", userLoginDto.Email);
            return null;
        }
        if (!(await _userManager.CheckPasswordAsync(_user, userLoginDto.Password)))
        {
            _logger.LogWarning("Login Failed: incorrect password provided for email ({email})", userLoginDto.Email);
            return null;
        }

        return new UserAuthResponseDto { UserID = _user.Id, Token = await _GenerateToken(), RefreshToken = await _CreateRefreshToken() };
    }
    public async Task<IEnumerable<IdentityError>> Register(UserRegisterDto userRegisterDto)
    {
        _logger.LogInformation("Registration Attempt with email: {email}", userRegisterDto.Email);

        _user = _mapper.Map<User>(userRegisterDto);
        _user.UserName = userRegisterDto.Email;

        var result = await _userManager.CreateAsync(_user, userRegisterDto.Password);

        if (result.Succeeded)
            await _userManager.AddToRoleAsync(_user, "User");
        else
            _logger.LogWarning("Registration Failed: user with email ({email}) generated the following errors ({errors})", userRegisterDto.Email, JsonConvert.SerializeObject(result.Errors));

        return result.Errors;
    }
    public async Task<UserAuthResponseDto> VerifyRefreshToken(UserAuthResponseDto request)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenContent = tokenHandler.ReadJwtToken(request.Token);
        var username = tokenContent.Claims.ToList().FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub)?.Value;

        _user = await _userManager.FindByNameAsync(username);
        if (_user is null || _user.Id != request.UserID)
        {
            _logger.LogWarning("Verify Refresh Token Failed: user could not be found or user id does not match token");
            return null;
        }


        var isValidToken = await _userManager.VerifyUserTokenAsync(_user, _loginProvider, _refreshToken, request.Token);
        if (!isValidToken)
        {
            _logger.LogWarning("Verify Refresh Token Failed: invalid token provided for user id ({userId})", request.UserID);
            await _userManager.UpdateSecurityStampAsync(_user);
            return null;
        }

        var token = await _GenerateToken();
        return new UserAuthResponseDto
        {
            UserID = _user.Id,
            Token = token,
            RefreshToken = await _CreateRefreshToken()
        };
    }
    #endregion

    #region Private Methods
    private async Task<string> _GenerateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSettings:Key"]));
        var cresentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(_user);
        var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();
        var userClaims = await _userManager.GetClaimsAsync(_user);

        var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _user.Email)

            }.Union(roleClaims).Union(userClaims);

        var token = new JwtSecurityToken(
                issuer: _config["JWTSettings:Issuer"],
                audience: _config["JWTSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToInt32(_config["JWTSettings:DurationInHours"])),
                signingCredentials: cresentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private async Task<string> _CreateRefreshToken()
    {
        await _userManager.RemoveAuthenticationTokenAsync(_user, _loginProvider, _refreshToken);

        var newRefreshToken = await _userManager.GenerateUserTokenAsync(_user, _loginProvider, _refreshToken);
        await _userManager.SetAuthenticationTokenAsync(_user, _loginProvider, _refreshToken, newRefreshToken);

        return newRefreshToken;
    }
    #endregion
}
