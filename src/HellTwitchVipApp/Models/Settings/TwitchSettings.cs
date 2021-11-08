namespace HellTwitchVipApp.Models.Settings
{
    public class TwitchSettings
    {
        public string Channel { get; set; }
        public string RedirectUri { get; set; }
        public ClientSettings ClientSettings { get; set; }
        public BotSettings BotSettings { get; set; }
        public GiftValue GiftValue { get; set; }
    }
    public class ClientSettings
    {
        public string Id { get; set; }
        public string Secret { get; set; }
    }
    public class BotSettings
    {
        public string UserName { get; set; }
        public string OAuth { get; set; }
    }
    public class GiftValue
    {
        public int Tier1 { get; set; }
        public int Tier2 { get; set; }
        public int Tier3 { get; set; }
    }
}
