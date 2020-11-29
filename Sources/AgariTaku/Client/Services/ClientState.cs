using AgariTaku.Client.State;
using AgariTaku.Shared.Common;
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

        public ClientState(IConfiguration configuration)
        {
            AckTicks = new(configuration);
            ServerTickBuffer = new(configuration);
            ClientTickBuffer = new(configuration);

            Player = TickSource.East; // TODO[4-player] Use actual in-game wind
            CurrentTick = -1;
        }
    }
}
