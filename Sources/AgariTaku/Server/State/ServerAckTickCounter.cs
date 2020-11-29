using AgariTaku.Shared.Common;
using AgariTaku.Shared.Types;

namespace AgariTaku.Server.State
{
    // Ticks acked per client per receiver. For server, this is ticks received. (And [0, 0] is the internal tick counter.)
    public class ServerAckTickCounter
    {
        private readonly int[,] _counter;

        public int this[TickSource client, TickSource source]
        {
            get => _counter[(int)client, (int)source];
            set => _counter[(int)client, (int)source] = value;
        }

        public ServerAckTickCounter(IConfiguration configuration)
        {
            _counter = new int[1 + configuration.PlayersPerGame, 1 + configuration.PlayersPerGame];
            for (int i = 0; i < 1 + configuration.PlayersPerGame; i++)
            {
                for (int j = 0; j < 1 + configuration.PlayersPerGame; j++)
                {
                    _counter[i, j] = -1;
                }
            }
        }
    }
}
