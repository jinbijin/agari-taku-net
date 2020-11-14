using AgariTaku.Shared.Types;

namespace AgariTaku.Shared.Messages
{
    public class ServerGameTick
    {
        public TickSource Player { get; init; }
        public int TickNumber { get; init; }
        public object Inputs { get; init; } = new();
    }
}
