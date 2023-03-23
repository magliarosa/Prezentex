using Prezentex.Domain.Entities;
using Prezentex.Domain.Identity;

namespace Prezentex.Application.Common.Interfaces.Identity
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> LoginWithPasswordAsync(string email, string password);
        Task<AuthenticationResult> RegisterWithPasswordAsync(string email, string password, string userName, string displayName);
        Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
        event EventHandler<User> UserRegistered;
    }
}
