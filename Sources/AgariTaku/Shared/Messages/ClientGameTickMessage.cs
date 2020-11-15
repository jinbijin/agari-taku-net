using System.Collections.Generic;
using System.Linq;

namespace AgariTaku.Shared.Messages
{
    public class ClientGameTickMessage
    {
        /// <summary>
        /// From each contributor, the last acknowledged tick.
        /// </summary>
        public int[] AckTick { get; init; } = new int[5];
        /// <summary>
        /// Ticks to (re)send. Must be in ascending tick order.
        /// </summary>
        public IEnumerable<ClientGameTick> Ticks { get; init; } = Enumerable.Empty<ClientGameTick>();
    }
}
