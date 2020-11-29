using AgariTaku.Server.Types;
using AgariTaku.Shared.Common;
using AgariTaku.Shared.Types;
using System;
using System.Collections.Generic;

namespace AgariTaku.Server.State
{
    public class GameConnectionManager
    {
        private readonly IConfiguration _configuration;
        private readonly object _lock = new();
        private readonly Dictionary<string, GameConnection> _connectionsById = new();
        private readonly Dictionary<TickSource, GameConnection> _connectionsBySource = new();

        public event Action<IReadOnlyCollection<string>>? OnGameFull;

        public GameConnectionManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Connect(string connectionId, TickSource source)
        {
            GameConnection connection = new()
            {
                ConnectionId = connectionId,
                Source = source,
            };
            lock (_lock)
            {
                if (_connectionsBySource.TryGetValue(source, out GameConnection? existing))
                {
                    existing.ConnectionId = connectionId;
                    _connectionsById.Add(connectionId, existing);
                }
                else
                {
                    _connectionsById.Add(connectionId, connection);
                    _connectionsBySource.Add(source, connection);
                }

                if (_connectionsById.Count == _configuration.PlayersPerGame && _connectionsBySource.Count == _configuration.PlayersPerGame)
                {
                    OnGameFull?.Invoke(_connectionsById.Keys);
                }
            }
        }

        public void Disconnect(string connectionId)
        {
            lock (_lock)
            {
                if (_connectionsById.TryGetValue(connectionId, out GameConnection? existing))
                {
                    existing.ConnectionId = null;
                    _connectionsById.Remove(connectionId);
                }
            }
        }

        public IReadOnlyCollection<GameConnection> GetActiveConnections()
        {
            lock (_lock)
            {
                return _connectionsById.Values;
            }
        }

        public TickSource? GetSource(string connectionId)
        {
            lock (_lock)
            {
                _connectionsById.TryGetValue(connectionId, out GameConnection? existing);
                return existing?.Source;
            }
        }

        public string? GetConnectionId(TickSource source)
        {
            lock (_lock)
            {
                _connectionsBySource.TryGetValue(source, out GameConnection? existing);
                return existing?.ConnectionId;
            }
        }
    }
}
