using Microsoft.IdentityModel.Tokens;
using Prezentex.Api.Entities;
using Prezentex.Api.Options;
using Prezentex.Api.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Prezentex.Api.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IUsersRepository _usersRepository;
        private readonly JwtSettings _jwtSettings;

        public IdentityService(IFacebookAuthService facebookAuthService, IUsersRepository usersRepository, JwtSettings jwtSettings)
        {
            _facebookAuthService = facebookAuthService;
            _usersRepository = usersRepository;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken)
        {
            var validatedTokenResult = await _facebookAuthService.ValidateAccessTokenAsync(accessToken);
            if (!validatedTokenResult.Data.IsValid)
            {
                return new AuthenticationResult
                {
                    Errors = new[] { "Invalid Facebook token" }
                };
            }

            var userInfo = await _facebookAuthService.GetUserInfoAsync(accessToken);

            var user = await _usersRepository.GetUserByEmailAsync(userInfo.Email);

            if(user == null)
            {
                var newUser = new User
                {
                    CreatedDate = DateTimeOffset.UtcNow,
                    Email = userInfo.Email,
                    Username = userInfo.Email,
                    UpdatedDate = DateTimeOffset.UtcNow,
                    Id = Guid.NewGuid()
                };

                await _usersRepository.CreateUserAsync(newUser);

                return await GenerateAuthenticationResultForUserAsync(newUser);
            }

            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(User newUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, newUser.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, newUser.Email),
                    new Claim("id", newUser.Id.ToString())

                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}
