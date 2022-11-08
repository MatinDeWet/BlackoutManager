using BlackoutManager.API.SERVICE;
using BlackoutManager.DATA.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace BlackoutManager.API.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        #region Dependency Injection / CTOR
        private readonly IAuthService _authManager;

        public AccountController(IAuthService authManager)
        {
            _authManager = authManager;
        }
        #endregion

        #region Public Mehtods

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult> Register([FromBody] UserRegisterDto UserRegisterDto)
        {
            var errors = await _authManager.Register(UserRegisterDto);
            if (errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return BadRequest();
            }
            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDto UserLoginDto)
        {
            var authResponse = await _authManager.Login(UserLoginDto);
            if (authResponse is null)
                return Unauthorized();
            return Ok(authResponse);
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<ActionResult> RefreshToken([FromBody] UserAuthResponseDto request)
        {
            var authResponse = await _authManager.VerifyRefreshToken(request);
            if (authResponse is null)
                return Unauthorized();
            return Ok(authResponse);
        }
        #endregion
    }
}
