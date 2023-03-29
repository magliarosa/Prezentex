using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prezentex.Application.Common.Interfaces.Facebook;
using Prezentex.Application.Common.Interfaces.Identity;
using Prezentex.Application.Common.Interfaces.Repositories;
using Prezentex.Domain.Entities;
using Prezentex.Domain.Identity;
using Prezentex.Domain.Options;
using Prezentex.Infrastructure.Persistence.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Prezentex.Infrastructure.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IUsersRepository _usersRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly EntitiesDbContext _context;
        private readonly UserManager<User> _userManager;

        public event EventHandler<User> UserRegistered;

        public IdentityService(
            IFacebookAuthService facebookAuthService,
            IUsersRepository usersRepository,
            JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters,
            EntitiesDbContext context,
            UserManager<User> userManager)
        {
            _facebookAuthService = facebookAuthService;
            _usersRepository = usersRepository;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            _context = context;
            _userManager = userManager;
        }

        public async Task<AuthenticationResult> LoginWithPasswordAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthenticationResult { Success = false, Errors = new List<string>() { "User not found" } };
            if (!await _userManager.CheckPasswordAsync(user, password))
                return new AuthenticationResult { Success = false, Errors = new List<string>() { "Incorrect password" } };

            var authResult = await GenerateAuthenticationResultForUserAsync(user);
            return authResult;
        }
        public async Task<AuthenticationResult> RegisterWithPasswordAsync(string email, string password, string userName, string displayName)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == userName))
            {
                return new AuthenticationResult { Success = false, Errors = new List<string>() { "Username is already taken" } };
            }

            if (await _userManager.Users.AnyAsync(x => x.Email == email))
            {
                return new AuthenticationResult { Success = false, Errors = new List<string>() { "Email is already taken" } };
            }

            var user = new User
            {
                DisplayName = displayName,
                Email = email,
                UserName = userName
            };
            
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return new AuthenticationResult { Success = false, Errors = new List<string>() { "User register failed" } };
            }

            return await GenerateAuthenticationResultForUserAsync(user);
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

            var user = await _userManager.FindByEmailAsync(userInfo.Email);

            if (user == null)
            {
                var newUser = new User
                {
                    Email = userInfo.Email,
                    UserName = userInfo.Email,
                    DisplayName = userInfo.Email,
                    Id = Guid.NewGuid()
                };

                await _usersRepository.CreateUserAsync(newUser);

                OnUserRegistered(newUser);

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
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new[] { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await _context.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not exists" } };
            }

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has expired" } };
            }

            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token has been invalidated" } };
            }

            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new[] { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;
            _context.RefreshTokens.Update(storedRefreshToken);
            await _context.SaveChangesAsync();

            var user = await _usersRepository.GetUserAsync(Guid.Parse(validatedToken.Claims.Single(x => x.Type == "id").Value));
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName)
                }),
                Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecutityAlgorithm(validatedToken))
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
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase);
        }

        protected virtual void OnUserRegistered(User user)
        {
            UserRegistered?.Invoke(this, user);
        }

        public Task<AuthenticationResult> LoginWithPasswordAsync(string email, string password, string userName, string displayName)
        {
            throw new NotImplementedException();
        }

    }

}

