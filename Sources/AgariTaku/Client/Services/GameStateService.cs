using AgariTaku.Shared.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AgariTaku.Client.Services
{
    public class GameStateService
    {
        private HubConnection? _connection;
        private readonly Dictionary<int, Stopwatch> _stopwatches = new();
        private readonly Dictionary<int, long> _delays = new();

        public async Task StartConnection()
        {
            _connection = new HubConnectionBuilder().WithUrl($"https://localhost:5001/gamehub").Build();

            _connection.On<SyncTickMessage>("ServerSyncTick", ServerSyncTick);
            _connection.On<SyncTickMessage>("AckSyncTick", AckSyncTick);

            await _connection.StartAsync();
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
        }

        public IDictionary<int, long> Delays => _delays;
    }
}
