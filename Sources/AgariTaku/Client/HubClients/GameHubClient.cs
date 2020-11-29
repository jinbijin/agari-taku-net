using AgariTaku.Shared.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AgariTaku.Client.HubClients
{
    public class GameHubClient : IGameHubClient
    {
        private readonly HubConnection _connection;

        public GameHubClient(Action ping, Action<SyncTickMessage> serverSyncTick, Action<SyncTickMessage> ackSyncTick, Action<ServerGameTickMessage> serverGameTick)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"https://localhost:44324/gamehub")
                .AddMessagePackProtocol()
                .Build();

            _connection.On("Ping", ping);
            _connection.On("ServerSyncTick", serverSyncTick);
            _connection.On("AckSyncTick", ackSyncTick);
            _connection.On("ServerGameTick", serverGameTick);
        }

        public Task AckPing() => _connection.InvokeAsync("AckPing");

        public Task ClientGameTick(ClientGameTickMessage message) => _connection.InvokeAsync("ClientGameTick", message);

        public Task ClientSyncTick(SyncTickMessage message) => _connection.InvokeAsync<SyncTickMessage>("ClientSyncTick", message);

        public Task StartConnection() => _connection.StartAsync();
    }
}
