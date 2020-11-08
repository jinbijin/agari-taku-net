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
            _gameStateManager.Connect(Context.ConnectionId);
            await base.OnConnectedAsync();
        }

        public async Task ClientSyncTick(SyncTickMessage message)
        {
            await Clients.Caller.AckSyncTick(message);
        }

        public async Task ClientGameTick(GameTickMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
