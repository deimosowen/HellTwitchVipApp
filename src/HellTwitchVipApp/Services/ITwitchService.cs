using HellTwitchVipApp.Models.Response;
using System.Threading.Tasks;
using TwitchLib.Api.Auth;

namespace HellTwitchVipApp.Services
{
    public interface ITwitchService
    {
        string GetAuthorizationCodeUrl();
        Task<AuthCodeResponse> GetAccessTokenFromCodeAsync(string code);
        Task<ValidateAccessTokenResponse> ValidateAccessTokenAsync(string token);
        Task<TwitchLibUserResponse> GetUserInfo(string token, string login);
        void Connect();
    }
}
