using AgariTaku.Server.Hubs;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Hubs;
using AgariTaku.Shared.Messages;
using AgariTaku.Shared.Types;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AgariTaku.Server.State
{
    public class GameStateManager
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly Dictionary<string, TickSource> _connectionIds;
        private Timer? _timer;

        private int[] _ticks = new int[5];

        public GameStateManager(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
            _connectionIds = new();
        }

        public void Connect(string connectionId)
        {
            _connectionIds.Add(connectionId, TickSource.East);
            if (_connectionIds.Count == Constants.PLAYERS_PER_GAME)
            {
                StartSync(_connectionIds.Keys);
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

        public void ReceiveClientTick(ClientGameTickMessage message, string connectionId)
        {
            foreach (ClientGameTick tick in message.Ticks)
            {
                _ticks[(short)_connectionIds[connectionId]] = Math.Max(tick.TickNumber, _ticks[(short)_connectionIds[connectionId]]);
            }
        }

        private void FinishSync()
        {
            _timer = new(state => HandleTick(), null, 1000, 1000 / Constants.TICKS_PER_SECOND);
        }

        private void HandleTick()
        {
            foreach (var key in _connectionIds.Keys)
            {
                _hubContext.Clients.Client(key).ServerGameTick(new()
                {
                    AckTick = _ticks[(short)_connectionIds[key]],
                    Ticks = new List<ServerGameTick>
                    {
                        new()
                        {
                            TickNumber = _ticks[(short)TickSource.Server],
                            Player = TickSource.Server,
                        }
                    }
                });
            }
            _ticks[(short)TickSource.Server]++;
        }
    }
}
