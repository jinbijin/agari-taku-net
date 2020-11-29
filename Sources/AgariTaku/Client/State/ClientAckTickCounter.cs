using AgariTaku.Shared.Common;
using AgariTaku.Shared.Types;

namespace AgariTaku.Client.State
{
    public class ClientAckTickCounter
    {
        private readonly int[] _counter;

        public int this[TickSource source]
        {
            get => _counter[(int)source];
            set => _counter[(int)source] = value;
        }

        public ClientAckTickCounter(IConfiguration configuration)
        {
            _counter = new int[1 + configuration.PlayersPerGame];
            for (int i = 0; i < 1 + configuration.PlayersPerGame; i++)
            {
                _counter[i] = -1;
            }
        }

        public int[] ToArray()
        {
            return _counter;
        }
    }
}
