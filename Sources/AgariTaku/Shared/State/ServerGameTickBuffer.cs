using AgariTaku.Shared.Common;
using AgariTaku.Shared.Messages;
using AgariTaku.Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public IReadOnlyCollection<ServerGameTick?> Slice(TickSource source, int index, int count)
        {
            if (count < 0 || count > _configuration.TickBufferSize)
            {
                throw new NotSupportedException("Count must fit in tick buffer.");
            }

            return Enumerable.Range(index, count).Select(i => this[source, i]).ToArray();
        }

        public ServerGameTickBuffer(IConfiguration configuration)
        {
            _configuration = configuration;
            _buffer = new ServerGameTick?[1 + _configuration.PlayersPerGame, _configuration.TickBufferSize];
        }
    }
}
