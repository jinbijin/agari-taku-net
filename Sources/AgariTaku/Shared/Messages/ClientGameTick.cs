namespace AgariTaku.Shared.Messages
{
    public class ClientGameTick
    {
        public int TickNumber { get; init; }
        public object Inputs { get; init; } = new();
    }
}
