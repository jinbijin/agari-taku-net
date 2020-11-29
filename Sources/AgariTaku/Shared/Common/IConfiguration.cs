namespace AgariTaku.Shared.Common
{
    public interface IConfiguration
    {
        int PlayersPerGame { get; }
        int TicksPerSecond { get; }
        int SyncTickCount { get; }
        int TickBufferSize { get; }
    }
}
