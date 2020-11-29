using AgariTaku.Shared.Common;

namespace AgariTaku.Tests.Common
{
    public class TestConfiguration : IConfiguration
    {
        public int PlayersPerGame => 4;

        public int TicksPerSecond => 25;

        public int SyncTickCount => 2 * TicksPerSecond;

        public int TickBufferSize => 5;
    }
}
