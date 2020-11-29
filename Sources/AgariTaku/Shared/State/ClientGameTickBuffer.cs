using AgariTaku.Shared.Common;
using AgariTaku.Shared.Messages;

namespace AgariTaku.Shared.State
{
    /// <summary>
    /// A circular buffer recording client ticks
    /// </summary>
    public class ClientGameTickBuffer
    {
        private readonly IConfiguration _configuration;
        private readonly ClientGameTick?[] _buffer;

        public ClientGameTick? this[int i]
        {
            get => _buffer[i % _configuration.TickBufferSize];
            set => _buffer[i % _configuration.TickBufferSize] = value;
        }

        public ClientGameTickBuffer(IConfiguration configuration)
        {
            _configuration = configuration;
            _buffer = new ClientGameTick?[_configuration.TickBufferSize];
        }
    }
}
