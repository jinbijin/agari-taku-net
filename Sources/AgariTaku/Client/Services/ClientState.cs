using AgariTaku.Client.State;
using AgariTaku.Shared.State;
using AgariTaku.Shared.Types;

namespace AgariTaku.Client.Services
{
    public class ClientState
    {
        public TickSource Player { get; set; }

        public ClientAckTickCounter AckTicks { get; }
        public ServerGameTickBuffer ServerTickBuffer { get; }

        public int CurrentTick { get; set; }
        public ClientGameTickBuffer ClientTickBuffer { get; }

        public ClientState()
        {
            AckTicks = new();
            ServerTickBuffer = new();
            ClientTickBuffer = new();

            Player = TickSource.East; // TODO[4-player] Use actual in-game wind
            CurrentTick = -1;
        }
    }
}
