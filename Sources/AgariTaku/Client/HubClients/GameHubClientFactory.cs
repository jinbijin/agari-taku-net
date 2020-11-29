using AgariTaku.Shared.Messages;
using System;

namespace AgariTaku.Client.HubClients
{
    public class GameHubClientFactory : IGameHubClientFactory
    {
        public IGameHubClient Create(Action ping, Action<SyncTickMessage> serverSyncTick, Action<SyncTickMessage> ackSyncTick, Action<ServerGameTickMessage> serverGameTick)
        {
            return new GameHubClient(ping, serverSyncTick, ackSyncTick, serverGameTick);
        }
    }
}
