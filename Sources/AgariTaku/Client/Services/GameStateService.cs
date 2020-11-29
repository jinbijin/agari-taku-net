using AgariTaku.Client.HubClients;
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
        private readonly IConfiguration _configuration;
        private readonly IGameHubClientFactory _gameHubClientFactory;
        private readonly TickService _tickService;

        private IGameHubClient _connection;
        private readonly Dictionary<int, Stopwatch> _stopwatches = new();
        private readonly Dictionary<int, long> _delays = new();
        private Timer? _timer;

        public event Action? OnChange;

        public IDictionary<int, long> Delays => _delays;

        public long AverageDelay => _delays.Any(d => d.Key >= -20) ? _delays.Where(d => d.Key >= -20).Sum(d => d.Value) / _delays.Count(d => d.Key >= -20) : 0;

        public int CurrentTick => _tickService.CurrentTick;
        public int ServerTick => _tickService.ServerTick;
        public int EchoTick => _tickService.EchoTick;

        public GameStateService(IConfiguration configuration, IGameHubClientFactory gameHubClientFactory)
        {
            _configuration = configuration;
            _gameHubClientFactory = gameHubClientFactory;
            _tickService = new TickService(configuration);
        }

        public async Task StartConnection()
        {
            _connection = _gameHubClientFactory.Create(Ping, ServerSyncTick, AckSyncTick, ServerGameTick);

            await _connection.StartConnection();
        }

        public void Ping()
        {
            _connection.AckPing();
        }

        public void ServerSyncTick(SyncTickMessage message)
        {
            Stopwatch stopwatch = new();
            _stopwatches.Add(message.TickNumber, stopwatch);
            stopwatch.Start();
            _connection.ClientSyncTick(message);
        }

        public void AckSyncTick(SyncTickMessage message)
        {
            Stopwatch stopwatch = _stopwatches[message.TickNumber];
            stopwatch.Stop();
            _delays.Add(message.TickNumber, stopwatch.ElapsedMilliseconds);

            // TODO[sync-packet-loss] Packet loss handling during sync
            if (message.TickNumber == -1)
            {
                _timer = new(state => HandleTick(), null, 1000 - (AverageDelay / 2), 1000 / _configuration.TicksPerSecond);
            }
            OnChange?.Invoke();
        }

        public void ServerGameTick(ServerGameTickMessage message)
        {
            _tickService.ReceiveMessage(message);
            OnChange?.Invoke();
        }

        public void HandleTick()
        {
            _tickService.AddTick();
            _tickService.SendAccumulatedTicks(_connection);
            OnChange?.Invoke();
        }
    }
}
