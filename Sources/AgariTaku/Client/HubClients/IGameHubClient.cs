using AgariTaku.Shared.Messages;
using System.Threading.Tasks;

namespace AgariTaku.Client.HubClients
{
    public interface IGameHubClient
    {
        Task StartConnection();
        Task AckPing();
        Task ClientSyncTick(SyncTickMessage message);
        Task ClientGameTick(ClientGameTickMessage message);
    }
}
