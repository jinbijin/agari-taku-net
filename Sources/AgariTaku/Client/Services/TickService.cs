using AgariTaku.Client.State;
using AgariTaku.Shared.Messages;
using AgariTaku.Shared.State;
using AgariTaku.Shared.Types;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Linq;

namespace AgariTaku.Client.Services
{
    // TODO[sync-correction] keep track of sync with server, and correct if necessary
    public class TickService
    {
        private readonly object _lock = new();

        private readonly TickSource _player;

        private readonly ClientAckTickCounter _ackTicks;
        private readonly ServerGameTickBuffer _serverTickBuffer;

        private int _currentTick = -1;
        private ClientGameTickBuffer _clientTickBuffer;

        public int CurrentTick => _currentTick;
        public int ServerTick => _ackTicks[TickSource.Server];
        public int EchoTick => _ackTicks[_player];

        public TickService()
        {
            _player = TickSource.East; // TODO[4-player] Use actual in-game wind
            _ackTicks = new();
            _serverTickBuffer = new();
            _clientTickBuffer = new();
        }

        public void ReceiveMessage(ServerGameTickMessage message)
        {
            lock (_lock)
            {
                foreach (ServerGameTick tick in message.Ticks.Where(tick => tick.TickNumber > _ackTicks[tick.Player]))
                {
                    _ackTicks[tick.Player] = tick.TickNumber;
                    _serverTickBuffer[tick.Player, tick.TickNumber] = tick;
                }

                // TODO[disconnect-handling] Detect server disconnection, and abort connection
            }
        }

        public void AddTick()
        {
            lock (_lock)
            {
                int currentTick = _currentTick + 1;
                ClientGameTick tick = new()
                {
                    TickNumber = currentTick,
                    Inputs = new(),
                };
                _clientTickBuffer[currentTick] = tick;

                _currentTick++;
            }
        }

        public void SendAccumulatedTicks(HubConnection? connection)
        {
            lock (_lock)
            {
                List<ClientGameTick> ticks = new();
                for (int i = _ackTicks[_player] + 1; i < _currentTick; i++)
                {
                    ticks.Add(_clientTickBuffer[i]);
                }
                ClientGameTickMessage message = new()
                {
                    AckTick = _ackTicks.ToArray(),
                    Ticks = ticks,
                };
                connection.InvokeAsync<ClientGameTickMessage>("ClientGameTick", message);
            }
        }
    }
}
