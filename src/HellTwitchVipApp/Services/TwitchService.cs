using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HellTwitchVipApp.Data.Concrete;
using HellTwitchVipApp.Data.Repositories;
using HellTwitchVipApp.Models.Response;
using HellTwitchVipApp.Models.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace HellTwitchVipApp.Services
{
    public class TwitchService: ITwitchService
    {
        // private readonly IGiverRepository _giverRepository;
        //private readonly HellAppContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TwitchAPI _twitchApi;
        private TwitchClient _twitchClient;
        private TwitchSettings _twitchSettings;
        private readonly ILogger<TwitchService> _logger;
        public TwitchService(/*IGiverRepository giverRepository,*/ IServiceScopeFactory scopeFactory, IOptions<TwitchSettings> twitchSettings, ILogger<TwitchService> logger)
        {
            // _giverRepository = giverRepository;
            //_context = context;
            _scopeFactory = scopeFactory;
            _twitchSettings = twitchSettings.Value;
            _logger = logger;
            _twitchApi = new TwitchAPI
            {
                Settings =
                {
                    ClientId = _twitchSettings.ClientSettings.Id,
                    Secret = _twitchSettings.ClientSettings.Secret
                }
            };
        }

        public string GetAuthorizationCodeUrl()
        {
            
            return _twitchApi.Auth.GetAuthorizationCodeUrl(_twitchSettings.RedirectUri, 
                new List<AuthScopes>
                {
                    AuthScopes.Channel_Read,
                }, 
                false, Guid.NewGuid().ToString(),
                _twitchSettings.ClientSettings.Id);
        }

        public async Task<AuthCodeResponse> GetAccessTokenFromCodeAsync(string code)
        {
            return await _twitchApi.Auth.GetAccessTokenFromCodeAsync(code, _twitchSettings.ClientSettings.Secret,
                _twitchSettings.RedirectUri, _twitchSettings.ClientSettings.Id);
        }

        public async Task<ValidateAccessTokenResponse> ValidateAccessTokenAsync(string token)
        {
            return await _twitchApi.Auth.ValidateAccessTokenAsync(token);
        }

        public async Task<TwitchLibUserResponse> GetUserInfo(string token, string login)
        {
            var response = await _twitchApi.Helix.Users.GetUsersAsync(accessToken: token, logins: new List<string> { login });
            return response.Users.Select(s=> new TwitchLibUserResponse(s)).FirstOrDefault();
        }

        public void Connect(string token)
        {
            ConnectionCredentials credentials = new ConnectionCredentials(_twitchSettings.BotSettings.UserName, _twitchSettings.BotSettings.OAuth, disableUsernameCheck: true);
            
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            _twitchClient = new TwitchClient(customClient);
            _twitchClient.Initialize(credentials, _twitchSettings.Channal);
            _twitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            _twitchClient.OnConnected += Client_OnConnected;
            _twitchClient.OnMessageSent += Client_OnMessageSent;
            _twitchClient.OnLog += Client_OnLog;
            _twitchClient.OnGiftedSubscription += Client_OnGiftedSubscription;
            _twitchClient.OnContinuedGiftedSubscription += Client_OnContinuedGiftedSubscription;
            _twitchClient.Connect();
            _twitchClient.JoinChannel(_twitchSettings.Channal);

        }

        private void Client_OnContinuedGiftedSubscription(object sender, OnContinuedGiftedSubscriptionArgs e)
        {
            _logger.LogInformation($"Sent message: {e.ContinuedGiftedSubscription.DisplayName}");
        }

        private void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HellAppContext>();
                var giver = dbContext.GiftSubscriptionGivers.ToList();
            }
            _logger.LogInformation($"Sent message: {e.GiftedSubscription.Login}");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            //_logger.LogInformation($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }
        private void Client_OnMessageSent(object sender, OnMessageSentArgs e)
        {
            _logger.LogInformation($"Sent message: {e.SentMessage}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            _logger.LogInformation($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            _logger.LogInformation("Hey guys! I am a bot connected via TwitchLib!");
           // _twitchClient.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }
    }
}
