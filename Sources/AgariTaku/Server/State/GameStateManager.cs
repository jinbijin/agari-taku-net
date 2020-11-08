using AgariTaku.Server.Hubs;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;

namespace AgariTaku.Server.State
{
    public class GameStateManager
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly List<string> _connectionIds;

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

        public void StartSync(IReadOnlyCollection<string> connectionIds)
        {
            SyncTimer syncTimer = new SyncTimer(_hubContext, connectionIds);
            syncTimer.StartSync();
        }
    }
}
