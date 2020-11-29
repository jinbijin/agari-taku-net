using AgariTaku.Server.Hubs;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AgariTaku.Server.State
{
    public sealed class SyncTimer
    {
        private readonly IConfiguration _configuration;
        private Timer? _timer;
        private readonly IGameClient _recipients;

        private int _tickNumber;

        public event Action? FinishSync;

        public SyncTimer(IConfiguration configuration, IHubContext<GameHub, IGameClient> hubContext, IReadOnlyCollection<string> connectionIds)
        {
            _configuration = configuration;
            _recipients = hubContext.Clients.Clients(connectionIds);
        }

        public void StartSync()
        {
            _tickNumber = -_configuration.SyncTickCount;
            _timer = new(async state => await HandleTick(), null, 0, 1000 / _configuration.TicksPerSecond);
        }

        public async Task HandleTick()
        {
            _tickNumber++;
            if (_tickNumber >= 0)
            {
                _timer?.Dispose();
                FinishSync?.Invoke();
            }
            await _recipients.ServerSyncTick(new() { TickNumber = _tickNumber - 1 });
        }
    }
}
