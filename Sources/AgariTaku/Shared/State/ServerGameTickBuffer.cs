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
        private readonly IConfiguration _configuration;
        private readonly ServerGameTick?[,] _buffer; // Ticks stored per client;

        public ServerGameTick? this[TickSource source, int i]
        {
            get => _buffer[(int)source, i % _configuration.TickBufferSize];
            set => _buffer[(int)source, i % _configuration.TickBufferSize] = value;
        }

        public ServerGameTickBuffer(IConfiguration configuration)
        {
            _configuration = configuration;
            _buffer = new ServerGameTick?[1 + _configuration.PlayersPerGame, _configuration.TickBufferSize];
        }
    }
}
