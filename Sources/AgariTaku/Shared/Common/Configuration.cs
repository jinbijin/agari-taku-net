namespace AgariTaku.Shared.Common
{
    public class Configuration : IConfiguration
    {
        public int PlayersPerGame => 1;

        public int TicksPerSecond => 25;

        public int SyncTickCount => 2 * TicksPerSecond;

        public int TickBufferSize => 2 * TicksPerSecond;
    }
}
