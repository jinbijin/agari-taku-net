namespace AgariTaku.Shared.Messages
{
    public class StateMessage : ServerGameTickMessage
    {
        public object GameState { get; init; } = new();
    }
}
