using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HellTwitchVipApp.Data.Concrete;
using HellTwitchVipApp.Data.Entities.Models;
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
using SubscriptionPlan = TwitchLib.Client.Enums.SubscriptionPlan;

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
            var response =
                await _twitchApi.Helix.Users.GetUsersAsync(accessToken: token, logins: new List<string> { login });
            return response.Users.Select(s => new TwitchLibUserResponse(s)).FirstOrDefault();
        }

        public void Connect(string token)
        {
            var credentials = new ConnectionCredentials(_twitchSettings.BotSettings.UserName,
                _twitchSettings.BotSettings.OAuth, disableUsernameCheck: true);

            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            var customClient = new WebSocketClient(clientOptions);
            _twitchClient = new TwitchClient(customClient);
            _twitchClient.Initialize(credentials, _twitchSettings.Channel);
            _twitchClient.OnJoinedChannel += Client_OnJoinedChannel;
            _twitchClient.OnConnected += Client_OnConnected;
            _twitchClient.OnGiftedSubscription += Client_OnGiftedSubscription;
            _twitchClient.Connect();
            _twitchClient.JoinChannel(_twitchSettings.Channel);
        }

        private async void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HellAppContext>();
            var giver = dbContext.GiftSubscriptionGivers.SingleOrDefault(s => s.UserName.ToLower() == e.GiftedSubscription.Login.ToLower());
            if (giver is null)
            {
                await dbContext.GiftSubscriptionGivers.AddAsync(new GiftSubscriptionGiver()
                {
                    IsVip = false,
                    UserName = e.GiftedSubscription.DisplayName,
                    GiftCount = GetGiftValue(e.GiftedSubscription.MsgParamSubPlan),
                });
            }
            else
            {
                giver.GiftCount += GetGiftValue(e.GiftedSubscription.MsgParamSubPlan);
            }

            await dbContext.SaveChangesAsync();
        }

        private int GetGiftValue(SubscriptionPlan subscriptionPlan)
        {
            return subscriptionPlan switch
            {
                SubscriptionPlan.Tier1 => _twitchSettings.GiftValue.Tier1,
                SubscriptionPlan.Tier2 => _twitchSettings.GiftValue.Tier2,
                SubscriptionPlan.Tier3 => _twitchSettings.GiftValue.Tier3,
                _ => _twitchSettings.GiftValue.Tier1
            };
        }
        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            _logger.LogInformation($"Connected to {e.AutoJoinChannel}");
        }
        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            _logger.LogInformation("Hey guys!");
        }
    }
}
