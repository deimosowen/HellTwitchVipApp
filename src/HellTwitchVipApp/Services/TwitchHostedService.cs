using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HellTwitchVipApp.Services
{
    public sealed class TwitchHostedService: IHostedService, IDisposable
    {
        private readonly ITwitchService _twitchService;
        public TwitchHostedService(ITwitchService twitchService)
        {
            _twitchService = twitchService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _twitchService.Connect("");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            
        }
    }
}
