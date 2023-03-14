using Prezentex.Domain.External.Facebook;

namespace Prezentex.Application.Common.Interfaces.Facebook
{
    public interface IFacebookAuthService
    {
        Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken);
        Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken);
    }
}
