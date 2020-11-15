using AgariTaku.Server.State;
using AgariTaku.Shared.Hubs;
using AgariTaku.Shared.Messages;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace AgariTaku.Server.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        private readonly GameStateManager _gameStateManager;

        public GameHub(GameStateManager gameStateManager)
        {
            _gameStateManager = gameStateManager;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.Ping();
        }

        // TODO[ping-retry] Retry ping
        public async Task AckPing()
        {
            _gameStateManager.Connect(Context.ConnectionId);
        }

        public async Task ClientSyncTick(SyncTickMessage message)
        {
            await Clients.Caller.AckSyncTick(message);
        }

        public async Task ClientGameTick(ClientGameTickMessage message)
        {
            _gameStateManager.ReceiveClientTick(message, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _gameStateManager.Disconnect(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
