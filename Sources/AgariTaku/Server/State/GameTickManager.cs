using AgariTaku.Server.Hubs;
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
        private const int BUFFER_SIZE = 2 * Constants.TICKS_PER_SECOND;

        private readonly IHubContext<GameHub, IGameClient> _hubContext;
        private readonly object _lock = new();

        private int[,] _ackTicks; // Ticks acked per client per receiver. For server, this is ticks received. (So [0,0] is empty.)
        private ServerGameTick?[,] _tickBuffer; // Ticks stored per client;

        public GameTickManager(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
            _ackTicks = new int[1 + Constants.PLAYERS_PER_GAME, 1 + Constants.PLAYERS_PER_GAME];
            _tickBuffer = new ServerGameTick?[1 + Constants.PLAYERS_PER_GAME, BUFFER_SIZE];
        }

        public void ProcessClientMessage(ClientGameTickMessage message, TickSource source)
        {
            int[] _newAckTicks = new int[1 + Constants.PLAYERS_PER_GAME];
            message.AckTick.CopyTo(_newAckTicks, 0);

            lock (_lock)
            {
                for (int i = 0; i < 1 + Constants.PLAYERS_PER_GAME; i++)
                {
                    _ackTicks[(int)source, i] = _newAckTicks[i];
                }

                foreach (ClientGameTick tick in message.Ticks.Where(tick => tick.TickNumber > _ackTicks[(int)TickSource.Server, (int)source]))
                {
                    _tickBuffer[(int)source, tick.TickNumber % BUFFER_SIZE] = new ServerGameTick
                    {
                        Player = source,
                        TickNumber = tick.TickNumber,
                        Inputs = tick.Inputs,
                    };
                    _ackTicks[(int)TickSource.Server, (int)source] = tick.TickNumber;
                }

                // TODO if there is any client more than two seconds behind, disconnect them.
            }
        }
    }
}
