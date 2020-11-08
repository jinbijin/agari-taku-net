namespace AgariTaku.Shared.Messages
{
    public class StateMessage : GameTickMessage
    {
        public object GameState { get; init; } = new();
    }
}
