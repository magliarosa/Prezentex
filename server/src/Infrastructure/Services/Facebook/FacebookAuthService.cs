using Newtonsoft.Json;
using Prezentex.Application.Common.Interfaces.Facebook;
using Prezentex.Domain.External.Facebook;
using Prezentex.Domain.Options;

namespace Prezentex.Infrastructure.Services.Facebook
{
    public class FacebookAuthService : IFacebookAuthService
    {
        private const string TOKEN_VALIDATION_URL = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string TOKEN_USER_INFO_URL = "https://graph.facebook.com/me?fields=first_name,last_name,email,picture&access_token={0}";
        private readonly FacebookAuthSettings facebookAuthSettings;
        private readonly IHttpClientFactory httpClientFactory;

        public FacebookAuthService(FacebookAuthSettings facebookAuthSettings, IHttpClientFactory httpClientFactory)
        {
            this.facebookAuthSettings = facebookAuthSettings;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken)
        {
            var formattedUrl = string.Format(TOKEN_USER_INFO_URL, accessToken);
            var result = await httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(responseAsString);
        }

        public async Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken)
        {
            var formattedUrl = string.Format(TOKEN_VALIDATION_URL, accessToken, facebookAuthSettings.AppId, facebookAuthSettings.AppSecret);
            var result = await httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();
            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseAsString);
        }
    }
}
