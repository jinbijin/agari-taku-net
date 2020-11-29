using AgariTaku.Shared.Common;
using AgariTaku.Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IReadOnlyCollection<ClientGameTick?> Slice(int index, int count)
        {
            if (count < 0 || count > _configuration.TickBufferSize)
            {
                throw new NotSupportedException("Count must fit inside buffer size.");
            }

            return Enumerable.Range(index, count).Select(i => this[i]).ToArray();
        }

        public ClientGameTickBuffer(IConfiguration configuration)
        {
            _configuration = configuration;
            _buffer = new ClientGameTick?[_configuration.TickBufferSize];
        }
    }
}
