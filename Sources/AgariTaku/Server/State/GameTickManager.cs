using AgariTaku.Server.Hubs;
using AgariTaku.Server.Types;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Hubs;
using AgariTaku.Shared.Messages;
using AgariTaku.Shared.State;
using AgariTaku.Shared.Types;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;

namespace AgariTaku.Server.State
{
    public class GameTickManager
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly GameConnectionManager _connectionManager;
        private readonly object _lock = new();

        private ServerAckTickCounter _ackTicks; // Ticks acked per client per receiver. For server, this is ticks received. (And [0, 0] is the internal tick counter.)
        private ServerGameTickBuffer _tickBuffer; // Ticks stored per client;

        public GameTickManager(IHubContext<GameHub, IGameClient> hubContext, GameConnectionManager connectionManager)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _ackTicks = new();
            _tickBuffer = new();
        }

        public void ProcessClientMessage(ClientGameTickMessage message, TickSource source)
        {
            int[] _newAckTicks = new int[1 + Constants.PLAYERS_PER_GAME];
            message.AckTick.CopyTo(_newAckTicks, 0);

            lock (_lock)
            {
                for (int i = 0; i < 1 + Constants.PLAYERS_PER_GAME; i++)
                {
                    _ackTicks[source, (TickSource)i] = _newAckTicks[i];
                }

                foreach (ClientGameTick tick in message.Ticks.Where(tick => tick.TickNumber > _ackTicks[TickSource.Server, source]))
                {
                    _tickBuffer[source, tick.TickNumber] = new ServerGameTick
                    {
                        Player = source,
                        TickNumber = tick.TickNumber,
                        Inputs = tick.Inputs,
                    };
                    _ackTicks[TickSource.Server, source] = tick.TickNumber;
                }

                // TODO[disconnect-handling] if there is any client more than two seconds behind, disconnect them
                // TODO[input-takeover] and then have the server take over their inputs
            }
        }

        public void AddServerTick()
        {
            lock (_lock)
            {
                int currentTick = _ackTicks[TickSource.Server, TickSource.Server] + 1;
                _tickBuffer[TickSource.Server, currentTick] = new ServerGameTick
                {
                    Player = TickSource.Server,
                    TickNumber = currentTick,
                    Inputs = new(),
                };
                _ackTicks[TickSource.Server, TickSource.Server]++;
            }
        }

        public void SendAccumulatedTicks()
        {
            IReadOnlyCollection<GameConnection> connections = _connectionManager.GetActiveConnections();
            lock (_lock)
            {
                foreach (GameConnection connection in connections)
                {
                    List<ServerGameTick> ticks = new();
                    for (int i = 0; i < 1 + Constants.PLAYERS_PER_GAME; i++)
                    {
                        for (int j = _ackTicks[connection.Source, (TickSource)i] + 1; j <= _ackTicks[TickSource.Server, (TickSource)i]; j++)
                        {
                            ticks.Add(_tickBuffer[(TickSource)i, j]);
                        }
                    }
                    _hubContext.Clients.Client(connection.ConnectionId).ServerGameTick(new()
                    {
                        AckTick = _ackTicks[TickSource.Server, connection.Source],
                        Ticks = ticks,
                    });
                }
            }
        }
    }
}
