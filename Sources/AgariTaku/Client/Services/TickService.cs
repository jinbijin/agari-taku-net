using AgariTaku.Shared.Common;
using AgariTaku.Shared.Messages;
using AgariTaku.Shared.Types;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Linq;

namespace AgariTaku.Client.Services
{
    // TODO[sync-correction] keep track of sync with server, and correct if necessary
    // TODO[test] Write unit tests
    public class TickService
    {
        private readonly object _lock = new();

        private readonly ClientState _state;

        public int CurrentTick => _state.CurrentTick;
        public int ServerTick => _state.AckTicks[TickSource.Server];
        public int EchoTick => _state.AckTicks[_state.Player];

        public TickService(IConfiguration configuration) : this(configuration, new(configuration))
        {
        }

        public TickService(IConfiguration configuration, ClientState state)
        {
            _state = state;
        }

        public void ReceiveMessage(ServerGameTickMessage message)
        {
            lock (_lock)
            {
                _state.AckTicks[_state.Player] = message.AckTick;

                foreach (ServerGameTick tick in message.Ticks.Where(tick => tick.TickNumber > _state.AckTicks[tick.Player]))
                {
                    _state.AckTicks[tick.Player] = tick.TickNumber;
                    _state.ServerTickBuffer[tick.Player, tick.TickNumber] = tick;
                }

                // TODO[disconnect-handling] Detect server disconnection, and abort connection
            }
        }

        public void AddTick()
        {
            lock (_lock)
            {
                int currentTick = _state.CurrentTick + 1;
                ClientGameTick tick = new()
                {
                    TickNumber = currentTick,
                    Inputs = new(),
                };
                _state.ClientTickBuffer[currentTick] = tick;

                _state.CurrentTick++;
            }
        }

        public void SendAccumulatedTicks(HubConnection? connection)
        {
            lock (_lock)
            {
                List<ClientGameTick> ticks = new();
                for (int i = _state.AckTicks[_state.Player] + 1; i <= _state.CurrentTick; i++)
                {
                    ticks.Add(_state.ClientTickBuffer[i]);
                }
                ClientGameTickMessage message = new()
                {
                    AckTick = _state.AckTicks.ToArray(),
                    Ticks = ticks,
                };
                connection.InvokeAsync<ClientGameTickMessage>("ClientGameTick", message);
            }
        }
    }
}
