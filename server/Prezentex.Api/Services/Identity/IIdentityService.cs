using Prezentex.Api.Entities;

namespace Prezentex.Api.Services.Identity
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
        event EventHandler<User> UserRegistered;
    }
}
