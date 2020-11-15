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
        /// Ticks to (re)send. Must be in ascending tick order per player.
        /// </summary>
        public IEnumerable<ServerGameTick> Ticks { get; init; } = Enumerable.Empty<ServerGameTick>();
    }
}
