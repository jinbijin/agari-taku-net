using AgariTaku.Shared.Types;

namespace AgariTaku.Server.Types
{
    public class GameConnection
    {
        public string? ConnectionId { get; set; }
        public TickSource Source { get; init; }
    }
}
