using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandGameOver : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            Done = true;
            client.StateMachine.SetNewCurrentState(client, new GameStateGameOver());
            client.CommandQueue.ClearCommands();
        }
    }
}
