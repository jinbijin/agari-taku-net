using AgariTaku.Shared.Messages;
using System.Threading.Tasks;

namespace AgariTaku.Shared.Hubs
{
    public interface IGameClient
    {
        Task Ping();
        Task ServerSyncTick(SyncTickMessage message);
        Task AckSyncTick(SyncTickMessage message);
        Task ClientGameTick(ClientGameTickMessage message);
        Task ServerGameTick(ServerGameTickMessage message);
        Task SyncState(StateMessage message);
    }
}
