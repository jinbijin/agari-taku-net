using AgariTaku.Server.Hubs;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;

namespace AgariTaku.Server.State
{
    public sealed class SyncTimer
    {
        private readonly System.Timers.Timer _timer;
        private readonly IGameClient _recipients;

        private int _tickNumber;

        public SyncTimer(IHubContext<GameHub, IGameClient> hubContext, IReadOnlyCollection<string> connectionIds)
        {
            _timer = new System.Timers.Timer(1000 / Constants.TICKS_PER_SECOND);
            _recipients = hubContext.Clients.Clients(connectionIds);
        }

        public void StartSync()
        {
            _tickNumber = -Constants.SYNC_TICK_COUNT;
            _timer.Elapsed += (x, y) => HandleTick();
            _timer.Start();
        }

        public void HandleTick()
        {
            _recipients.ServerSyncTick(new() { TickNumber = _tickNumber });
            _tickNumber++;
            if (_tickNumber >= 0)
            {
                _timer.Stop();
            }
        }
    }
}
