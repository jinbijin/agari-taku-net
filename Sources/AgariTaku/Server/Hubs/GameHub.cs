using AgariTaku.Shared.Hubs;
using AgariTaku.Shared.Messages;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace AgariTaku.Server.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        public async Task ClientSyncTick(SyncTickMessage message)
        {
            throw new NotImplementedException();
        }

        public async Task ClientGameTick(GameTickMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
