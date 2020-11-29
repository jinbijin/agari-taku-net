using System.Collections.Generic;
using System.Linq;

namespace AgariTaku.Shared.Messages
{
    public class ServerGameTickMessage
    {
        /// <summary>
        /// The last acknowledged tick from the recipient to the server.
        /// </summary>
        public int AckTick { get; init; }
        /// <summary>
        /// Ticks to (re)send. Must be in ascending tick order per player.
        /// </summary>
        public IEnumerable<ServerGameTick> Ticks { get; init; } = Enumerable.Empty<ServerGameTick>();
    }
}
