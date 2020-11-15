using AgariTaku.Shared.Common;
using AgariTaku.Shared.Messages;
using AgariTaku.Shared.Types;

namespace AgariTaku.Shared.State
{
    /// <summary>
    /// A circular buffer recording server ticks
    /// </summary>
    public class ServerGameTickBuffer
    {
        private readonly ServerGameTick?[,] _buffer; // Ticks stored per client;

        public ServerGameTick? this[TickSource source, int i]
        {
            get => _buffer[(int)source, i % Constants.TICK_BUFFER_SIZE];
            set => _buffer[(int)source, i % Constants.TICK_BUFFER_SIZE] = value;
        }

        public ServerGameTickBuffer()
        {
            _buffer = new ServerGameTick?[1 + Constants.PLAYERS_PER_GAME, Constants.TICK_BUFFER_SIZE];
        }
    }
}
