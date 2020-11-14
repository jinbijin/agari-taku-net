using System.Collections.Generic;
using System.Linq;

namespace AgariTaku.Shared.Messages
{
    public class ServerGameTickMessage
    {
        /// <summary>
        /// From each contributor, the last acknowledged tick.
        /// </summary>
        public int AckTick { get; init; }
        /// <summary>
        /// Ticks to (re)send.
        /// </summary>
        public IEnumerable<ServerGameTick> Ticks { get; init; } = Enumerable.Empty<ServerGameTick>();
    }
}
