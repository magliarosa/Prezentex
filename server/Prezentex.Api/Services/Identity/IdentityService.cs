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
        private readonly TokenValidationParameters _tokenValidationParameters;

        public IdentityService(IFacebookAuthService facebookAuthService, IUsersRepository usersRepository, JwtSettings jwtSettings, TokenValidationParameters tokenValidationParameters)
        {
            _facebookAuthService = facebookAuthService;
            _usersRepository = usersRepository;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
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

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "Invalid token" } };
            }

            var expiryDateUnix = 
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DatetTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix)
                .Subtract(_jwtSettings.TokenLifetime);

            if(expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

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
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if(!IsJwtWithValidSecutityAlgorithm(validatedToken))
                {
                    return null;
                };
                return principal;
            }
            catch
            {

                throw null;
            }
        }

        private bool IsJwtWithValidSecutityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
