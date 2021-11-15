using System;
using System.Collections.Generic;
using System.Text;

namespace TheParty_v2
{
    class CommandClearQueue : Command<TheParty>
    {
        public override void Update(TheParty client, float deltaTime)
        {
            client.CommandQueue.ClearCommands();
            Done = true;
        }
    }
}
