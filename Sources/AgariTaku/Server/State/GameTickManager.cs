using AgariTaku.Server.Hubs;
using AgariTaku.Server.Types;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Hubs;
using AgariTaku.Shared.Messages;
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

        private readonly GameState _state;

        public GameTickManager(IHubContext<GameHub, IGameClient> hubContext, GameConnectionManager connectionManager) : this(hubContext, connectionManager, new())
        {
        }

        public GameTickManager(IHubContext<GameHub, IGameClient> hubContext, GameConnectionManager connectionManager, GameState state)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _state = state;
        }

        public void ProcessClientMessage(ClientGameTickMessage message, TickSource source)
        {
            int[] _newAckTicks = new int[1 + Constants.PLAYERS_PER_GAME];
            message.AckTick.CopyTo(_newAckTicks, 0);

            lock (_lock)
            {
                for (int i = 0; i < 1 + Constants.PLAYERS_PER_GAME; i++)
                {
                    _state.AckTicks[source, (TickSource)i] = _newAckTicks[i];
                }

                foreach (ClientGameTick tick in message.Ticks.Where(tick => tick.TickNumber > _state.AckTicks[TickSource.Server, source]))
                {
                    _state.TickBuffer[source, tick.TickNumber] = new ServerGameTick
                    {
                        Player = source,
                        TickNumber = tick.TickNumber,
                        Inputs = tick.Inputs,
                    };
                    _state.AckTicks[TickSource.Server, source] = tick.TickNumber;
                }

                // TODO[disconnect-handling] if there is any client more than two seconds behind, disconnect them
                // TODO[input-takeover] and then have the server take over their inputs
            }
        }

        public void AddServerTick()
        {
            lock (_lock)
            {
                int currentTick = _state.AckTicks[TickSource.Server, TickSource.Server] + 1;
                _state.TickBuffer[TickSource.Server, currentTick] = new ServerGameTick
                {
                    Player = TickSource.Server,
                    TickNumber = currentTick,
                    Inputs = new(),
                };
                _state.AckTicks[TickSource.Server, TickSource.Server]++;
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
                        for (int j = _state.AckTicks[connection.Source, (TickSource)i] + 1; j <= _state.AckTicks[TickSource.Server, (TickSource)i]; j++)
                        {
                            ticks.Add(_state.TickBuffer[(TickSource)i, j]);
                        }
                    }
                    _hubContext.Clients.Client(connection.ConnectionId).ServerGameTick(new()
                    {
                        AckTick = _state.AckTicks[TickSource.Server, connection.Source],
                        Ticks = ticks,
                    });
                }
            }
        }
    }
}
