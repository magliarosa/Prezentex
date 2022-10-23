using Prezentex.Api.Entities;
using Prezentex.Api.Repositories;

namespace Prezentex.Api.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly IUsersRepository _usersRepository;

        public IdentityService(IFacebookAuthService facebookAuthService, IUsersRepository usersRepository)
        {
            _facebookAuthService = facebookAuthService;
            _usersRepository = usersRepository;
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

            return await GenerateAuthenticationResultForUserAsync(newUser);

        }
    }
}
