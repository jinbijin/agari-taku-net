using AgariTaku.Shared.Common;
using AgariTaku.Shared.Messages;
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

        private int[] _ackTicks;
        private ServerGameTick?[,] _serverTickBuffer;

        private int _currentTick = -1;
        private ClientGameTick?[] _clientTickBuffer;

        public int CurrentTick => _currentTick;
        public int ServerTick => _ackTicks[0];
        public int EchoTick => _ackTicks[1]; // TODO[4-player] use logged wind

        public TickService()
        {
            _ackTicks = new int[1 + Constants.PLAYERS_PER_GAME];
            for (int i = 0; i < 1 + Constants.PLAYERS_PER_GAME; i++)
            {
                _ackTicks[i] = -1;
            }
            _serverTickBuffer = new ServerGameTick?[1 + Constants.PLAYERS_PER_GAME, Constants.TICK_BUFFER_SIZE];
            _clientTickBuffer = new ClientGameTick?[Constants.TICK_BUFFER_SIZE];
        }

        public void ReceiveMessage(ServerGameTickMessage message)
        {
            lock (_lock)
            {
                foreach (ServerGameTick tick in message.Ticks.Where(tick => tick.TickNumber > _ackTicks[(int)tick.Player]))
                {
                    _ackTicks[(int)tick.Player] = tick.TickNumber;
                    _serverTickBuffer[(int)tick.Player, tick.TickNumber % Constants.TICK_BUFFER_SIZE] = tick;
                }
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
                _clientTickBuffer[currentTick % Constants.TICK_BUFFER_SIZE] = tick;

                _currentTick++;
            }
        }

        public void SendAccumulatedTicks(HubConnection? connection)
        {
            lock (_lock)
            {
                List<ClientGameTick> ticks = new();
                for (int i = _ackTicks[(int)TickSource.East] + 1; i < _currentTick; i++) // TODO[4-player] use logged wind
                {
                    ticks.Add(_clientTickBuffer[i % Constants.TICK_BUFFER_SIZE]);
                }
                ClientGameTickMessage message = new()
                {
                    AckTick = _ackTicks,
                    Ticks = ticks,
                };
                connection.InvokeAsync<ClientGameTickMessage>("ClientGameTick", message);
            }
        }
    }
}
