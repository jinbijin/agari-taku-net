﻿using AgariTaku.Shared.Messages;
using System;

namespace AgariTaku.Client.HubClients
{
    public interface IGameHubClientFactory
    {
        IGameHubClient Create(Action ping, Action<SyncTickMessage> serverSyncTick, Action<SyncTickMessage> ackSyncTick, Action<ServerGameTickMessage> serverGameTick);
    }
}
