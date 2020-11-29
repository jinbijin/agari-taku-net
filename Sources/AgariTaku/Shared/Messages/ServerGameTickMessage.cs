using System.Collections.Generic;
using System.Linq;

namespace AgariTaku.Shared.Messages
{
    public class ServerGameTickMessage
    {
        /// <summary>
        /// Ticks to (re)send. Must be in ascending tick order per player.
        /// </summary>
        public IEnumerable<ServerGameTick> Ticks { get; init; } = Enumerable.Empty<ServerGameTick>();
    }
}
