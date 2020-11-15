using AgariTaku.Shared.Common;
using AgariTaku.Shared.Messages;

namespace AgariTaku.Shared.State
{
    /// <summary>
    /// A circular buffer recording client ticks
    /// </summary>
    public class ClientGameTickBuffer
    {
        private readonly ClientGameTick?[] _buffer;

        public ClientGameTick? this[int i]
        {
            get => _buffer[i % Constants.TICK_BUFFER_SIZE];
            set => _buffer[i % Constants.TICK_BUFFER_SIZE] = value;
        }

        public ClientGameTickBuffer()
        {
            _buffer = new ClientGameTick?[Constants.TICK_BUFFER_SIZE];
        }
    }
}
