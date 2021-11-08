using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace HellTwitchVipApp.Models.Response
{
    public class TwitchLibUserResponse : User
    {
        public TwitchLibUserResponse(User user) : base()
        {
            Id = user.Id;
            Login = user.Login;
            DisplayName = user.DisplayName;
            CreatedAt = user.CreatedAt;
            Type = user.Type;
            BroadcasterType = user.BroadcasterType;
            Description = user.Description;
            ProfileImageUrl = user.ProfileImageUrl;
            OfflineImageUrl = user.OfflineImageUrl;
            ViewCount = user.ViewCount;
            Email = user.Email; 
        }
    }
}
