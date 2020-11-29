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
        private readonly IConfiguration _configuration;

        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly GameConnectionManager _connectionManager;
        private readonly GameTickManager _tickManager;
        private Timer? _timer;

        public GameStateManager(IConfiguration configuration, IHubContext<GameHub, IGameClient> hubContext, GameConnectionManager connectionManager, GameTickManager tickManager)
        {
            _configuration = configuration;

            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _tickManager = tickManager;
            _connectionManager.OnGameFull += StartSync;
        }

        public void Connect(string connectionId)
        {
            _connectionManager.Connect(connectionId, TickSource.East); // TODO[4-player] assign randomly
        }

        public void Disconnect(string connectionId)
        {
            _connectionManager.Disconnect(connectionId);
            // Testing with one connection
            _timer?.Dispose();
        }

        public void StartSync(IReadOnlyCollection<string> connectionIds)
        {
            SyncTimer syncTimer = new SyncTimer(_configuration, _hubContext, connectionIds);
            syncTimer.FinishSync += FinishSync;
            syncTimer.StartSync();
        }

        public void ReceiveClientTick(ClientGameTickMessage message, string connectionId)
        {
            _tickManager.ProcessClientMessage(message, _connectionManager.GetSource(connectionId).Value);
        }

        private void FinishSync()
        {
            _timer = new(state => HandleTick(), null, 1000, 1000 / _configuration.TicksPerSecond);
        }

        private void HandleTick()
        {
            _tickManager.AddServerTick();
            _tickManager.SendAccumulatedTicks();
        }
    }
}
