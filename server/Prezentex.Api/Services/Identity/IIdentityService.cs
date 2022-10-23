namespace Prezentex.Api.Services.Identity
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken);
    }
}
