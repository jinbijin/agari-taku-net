using AgariTaku.Shared.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AgariTaku.Client.Services
{
    public class GameStateService
    {
        private HubConnection? _connection;
        private readonly Dictionary<int, Stopwatch> _stopwatches = new();
        private readonly Dictionary<int, long> _delays = new();

        public event Action? OnChange;

        public async Task StartConnection()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"https://localhost:44324/gamehub")
                .AddMessagePackProtocol()
                .Build();

            _connection.On("Ping", Ping);
            _connection.On<SyncTickMessage>("ServerSyncTick", ServerSyncTick);
            _connection.On<SyncTickMessage>("AckSyncTick", AckSyncTick);

            await _connection.StartAsync();
        }

        public void Ping()
        {
            _connection.InvokeAsync("AckPing");
        }

        public void ServerSyncTick(SyncTickMessage message)
        {
            _connection.InvokeAsync<SyncTickMessage>("ClientSyncTick", message);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            _stopwatches.Add(message.TickNumber, stopwatch);
        }

        public void AckSyncTick(SyncTickMessage message)
        {
            Stopwatch stopwatch = _stopwatches[message.TickNumber];
            stopwatch.Stop();
            _delays.Add(message.TickNumber, stopwatch.ElapsedMilliseconds);
            OnChange?.Invoke();
        }

        public IDictionary<int, long> Delays => _delays;

        public double AverageDelay => _delays.Any(d => d.Key >= -20) ? _delays.Where(d => d.Key >= -20).Sum(d => d.Value) / _delays.Count(d => d.Key >= -20) : 0;
    }
}
