using AgariTaku.Server.Hubs;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Hubs;
using AgariTaku.Shared.Messages;
using AgariTaku.Shared.Types;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading;

namespace AgariTaku.Server.State
{
    public class GameStateManager
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly List<string> _connectionIds;
        private Timer? _timer;

        private int _ticks;

        public GameStateManager(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
            _connectionIds = new();
        }

        public void Connect(string connectionId)
        {
            _connectionIds.Add(connectionId);
            if (_connectionIds.Count == Constants.PLAYERS_PER_GAME)
            {
                StartSync(_connectionIds);
            }
        }

        public void Disconnect(string connectionId)
        {
            _connectionIds.Remove(connectionId);
            // Testing with one connection
            _timer?.Dispose();
        }

        public void StartSync(IReadOnlyCollection<string> connectionIds)
        {
            SyncTimer syncTimer = new SyncTimer(_hubContext, connectionIds);
            syncTimer.FinishSync += FinishSync;
            syncTimer.StartSync();
        }

        private void FinishSync()
        {
            _timer = new(state => HandleTick(), null, 1000, 1000 / Constants.TICKS_PER_SECOND);
        }

        private void HandleTick()
        {
            _hubContext.Clients.All.ServerGameTick(new()
            {
                Ticks = new List<GameTick>
                {
                    new()
                    {
                        TickNumber = _ticks,
                        Player = TickSource.Server,
                    }
                }
            });
            _ticks++;
        }
    }
}
