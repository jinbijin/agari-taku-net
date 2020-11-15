﻿using AgariTaku.Shared.State;

namespace AgariTaku.Server.State
{
    public class GameState
    {
        public ServerAckTickCounter AckTicks { get; }
        public ServerGameTickBuffer TickBuffer { get; }

        public GameState()
        {
            AckTicks = new();
            TickBuffer = new();
        }
    }
}