using AgariTaku.Shared.Types;
using System.Collections.Generic;
using System.Linq;

namespace AgariTaku.Shared.Messages
{
    public class GameTickMessage
    {
        /// <summary>
        /// From each contributor, the last acknowledged tick.
        /// </summary>
        public int[] AckTick { get; init; } = new int[5];
        /// <summary>
        /// Ticks to (re)send.
        /// </summary>
        public IEnumerable<GameTick> Ticks { get; init; } = Enumerable.Empty<GameTick>();
    }
}
