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

        public ServerAckTickCounter()
        {
            _counter = new int[1 + Constants.PLAYERS_PER_GAME, 1 + Constants.PLAYERS_PER_GAME];
            for (int i = 0; i < 1 + Constants.PLAYERS_PER_GAME; i++)
            {
                for (int j = 0; j < 1 + Constants.PLAYERS_PER_GAME; j++)
                {
                    _counter[i, j] = -1;
                }
            }
        }
    }
}
