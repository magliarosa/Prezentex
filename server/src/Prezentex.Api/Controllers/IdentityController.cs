using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Prezentex.Api.Dtos;
using Prezentex.Api.DTOs;
using Prezentex.Application.Common.Interfaces.Identity;
using Prezentex.Domain.Entities;
using Prezentex.Domain.Identity;
using System.Security.Claims;

namespace Prezentex.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly UserManager<User> _userManager;

        public IdentityController(IIdentityService identityService, UserManager<User> userManager)
        {
            _identityService = identityService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoggedUserDto>> Login([FromBody]LoginDto loginDto)
        {
            var result = await _identityService.LoginWithPasswordAsync(loginDto.Email, loginDto.Password);

            if (result.Success)
            {
                return Ok(new AuthSuccessResponse
                {
                    Token = result.Token,
                    RefreshToken = result.RefreshToken
                });
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<LoggedUserDto>> Register(RegisterDto registerDto)
        {
            var result = await _identityService.RegisterWithPasswordAsync(
                registerDto.Email,
                registerDto.Password,
                registerDto.UserName,
                registerDto.DisplayName);

            if (result.Success)
            {
                return Ok(new AuthSuccessResponse
                {
                    Token = result.Token,
                    RefreshToken = result.RefreshToken
                });
            }

            return BadRequest(new { Errors = result.Errors });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<LoggedUserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            return Ok(user.AsLoggedUserDto());

        }

        [AllowAnonymous]
        [HttpPost("/auth/fb")]
        public async Task<IActionResult> FacebookAuth([FromBody] UserFacebookAuthRequestDto request)
        {
            var authResponse = await _identityService.LoginWithFacebookAsync(request.AccessToken);
            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [AllowAnonymous]
        [HttpPost("/refresh")]
        public async Task<IActionResult> AuthRefresh([FromBody] RefreshTokenRequestDto request)
        {
            var authResponse = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }
    }
}
