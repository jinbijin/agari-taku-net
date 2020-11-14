using AgariTaku.Shared.Common;
using AgariTaku.Shared.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AgariTaku.Client.Services
{
    public class GameStateService
    {
        private HubConnection? _connection;
        private readonly Dictionary<int, Stopwatch> _stopwatches = new();
        private readonly Dictionary<int, long> _delays = new();
        private Timer? _timer;

        public event Action? OnChange;

        public IDictionary<int, long> Delays => _delays;

        public long AverageDelay => _delays.Any(d => d.Key >= -20) ? _delays.Where(d => d.Key >= -20).Sum(d => d.Value) / _delays.Count(d => d.Key >= -20) : 0;

        public int CurrentTick { get; private set; }
        public int ServerTick { get; private set; }

        public async Task StartConnection()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"https://localhost:44324/gamehub")
                .AddMessagePackProtocol()
                .Build();

            _connection.On("Ping", Ping);
            _connection.On<SyncTickMessage>("ServerSyncTick", ServerSyncTick);
            _connection.On<SyncTickMessage>("AckSyncTick", AckSyncTick);
            _connection.On<ServerGameTickMessage>("ServerGameTick", ServerGameTick);

            await _connection.StartAsync();
        }

        public void Ping()
        {
            _connection.InvokeAsync("AckPing");
        }

        public void ServerSyncTick(SyncTickMessage message)
        {
            Stopwatch stopwatch = new();
            _stopwatches.Add(message.TickNumber, stopwatch);
            stopwatch.Start();
            _connection.InvokeAsync<SyncTickMessage>("ClientSyncTick", message);
        }

        public void AckSyncTick(SyncTickMessage message)
        {
            Stopwatch stopwatch = _stopwatches[message.TickNumber];
            stopwatch.Stop();
            _delays.Add(message.TickNumber, stopwatch.ElapsedMilliseconds);
            if (message.TickNumber == -1)
            {
                _timer = new(state => HandleTick(), null, 1000 - (AverageDelay / 2), 1000 / Constants.TICKS_PER_SECOND);
            }
            OnChange?.Invoke();
        }

        public void ServerGameTick(ServerGameTickMessage message)
        {
            ServerTick = message.Ticks.First().TickNumber;
            OnChange?.Invoke();
        }

        public void HandleTick()
        {
            ClientGameTickMessage message = new()
            {
                Ticks = new List<ClientGameTick>
                {
                    new()
                    {
                        TickNumber = CurrentTick,
                    }
                }
            };
            CurrentTick++;
            _connection.InvokeAsync<ClientGameTickMessage>("ClientGameTick", new() { });
            OnChange?.Invoke();
        }
    }
}
